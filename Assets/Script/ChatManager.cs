using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI; 

public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform chatContent;       
    public TMP_InputField messageInput; 
    public Button sendButton;
    public ScrollRect scrollRect;       

    [Header("Prefabs")]
    public GameObject userBubblePrefab; 
    public GameObject aiBubblePrefab;   

    private string apiUrl = "http://localhost:8080/api/chat"; 

    [System.Serializable]
    public class ChatRequest { public string player_id; public string message; }
    [System.Serializable]
    public class ChatResponse { public string answer; public string status; }


    void Start()
    {
        sendButton.onClick.AddListener(SendMessageToAI);
    }

    public void SendMessageToAI()
    {
        if (string.IsNullOrEmpty(messageInput.text)) return;

        string userMessage = messageInput.text;
        
        CreateChatBubble(userBubblePrefab, userMessage);
        
        messageInput.text = "";

        StartCoroutine(PostRequest(userMessage));
    }

    IEnumerator PostRequest(string message)
    {
        GameObject aiBubble = CreateChatBubble(aiBubblePrefab, "...");
        TMP_Text aiBubbleText = aiBubble.GetComponentInChildren<TMP_Text>();

        ChatRequest reqData = new ChatRequest { player_id = "Tester001", message = message };
        string json = JsonUtility.ToJson(reqData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                aiBubbleText.text = "<color=red>Error connecting to AI</color>";
            }
            else
            {
                var response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                
                aiBubbleText.text = response.answer;
                
                aiBubbleText.richText = true; 
            }
        }
        
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    GameObject CreateChatBubble(GameObject prefab, string text)
    {
        GameObject newBubble = Instantiate(prefab, chatContent);
        
        TMP_Text bubbleText = newBubble.GetComponentInChildren<TMP_Text>();
        bubbleText.text = text;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        return newBubble;
    }
}