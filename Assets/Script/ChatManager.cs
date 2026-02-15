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

    public GameObject chatWindow;
    public Button openButton;
    public Button closeButton;
    public static ChatManager instance;

    private string apiUrl = "http://localhost:8080/api/chat"; 

    [System.Serializable]
    public class ChatRequest { public string player_id; public string message; }

    [System.Serializable]
    public class ChatResponse { public string answer; public string status; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sendButton.onClick.AddListener(OnSendClick);

        openButton.onClick.AddListener(OpenChat);
        closeButton.onClick.AddListener(CloseChat);

        CloseChat();
    }

    void OnSendClick()
    {
        if (string.IsNullOrEmpty(messageInput.text)) return;

        string msg = messageInput.text;
        CreateBubble(userBubblePrefab, msg);
        messageInput.text = ""; 

        StartCoroutine(PostRequest(msg));
    }

    IEnumerator PostRequest(string message)
    {
        GameObject aiBubble = CreateBubble(aiBubblePrefab, "กำลังค้นข้อมูล...");
        TMP_Text aiText = aiBubble.GetComponentInChildren<TMP_Text>();

        ChatRequest req = new ChatRequest { player_id = "Player01", message = message };
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                aiText.text = "<color=red>Error: " + request.error + "</color>";
            }
            else
            {
                try 
                {
                    ChatResponse res = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    aiText.text = res.answer;
                }
                catch
                {
                    aiText.text = request.downloadHandler.text;
                }
            }
        }

        StartCoroutine(ForceScrollDown());
    }

    GameObject CreateBubble(GameObject prefab, string text)
    {
        GameObject newBubble = Instantiate(prefab, chatContent);
        TMP_Text bubbleText = newBubble.GetComponentInChildren<TMP_Text>();
        bubbleText.text = text;

        StartCoroutine(ForceScrollDown());

        return newBubble;
    }

    IEnumerator ForceScrollDown()
    {
        yield return new WaitForEndOfFrame();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
        
        yield return new WaitForEndOfFrame();
        
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void OpenChat()
    {
        chatWindow.SetActive(true);
        openButton.gameObject.SetActive(false);
    }

    void CloseChat()
    {
        chatWindow.SetActive(false);
        openButton.gameObject.SetActive(true);
    }
}