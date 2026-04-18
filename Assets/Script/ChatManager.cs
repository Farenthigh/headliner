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

    [Header("Window Control")]
    public GameObject chatWindow;
    public Button openButton;
    public Button closeButton;

    [Header("Typing Settings")]
    public float typingSpeed = 0.02f; 

    public static ChatManager instance;

    private string apiUrl = "http://localhost:8080/api/chat"; 
    private string currentPlayerID;

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

        InitializePlayerID();
    }

    void InitializePlayerID()
    {
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            currentPlayerID = PlayerPrefs.GetString("PlayerID");
        }
        else
        {
            currentPlayerID = System.Guid.NewGuid().ToString(); 
            PlayerPrefs.SetString("PlayerID", currentPlayerID);
            PlayerPrefs.Save();
        }
        Debug.Log("Current Player ID: " + currentPlayerID);
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

        sendButton.interactable = false;

        string msg = messageInput.text;
        CreateBubble(userBubblePrefab, msg); 
        messageInput.text = ""; 

        StartCoroutine(PostRequest(msg));
    }

    IEnumerator PostRequest(string message)
    {
        GameObject aiBubble = CreateBubble(aiBubblePrefab, "กำลังพิมพ์...");
        TMP_Text aiText = aiBubble.GetComponentInChildren<TMP_Text>();

        if (aiText == null)
        {
            Debug.LogError("Error: AI Bubble Prefab missing TMP_Text component!");
            sendButton.interactable = true;
            yield break;
        }

        ChatRequest req = new ChatRequest { player_id = currentPlayerID, message = message };
        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                aiText.text = $"<color=red>Error: {request.error}</color>";
            }
            else
            {
                try 
                {
                    ChatResponse res = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    
                    if (res != null && !string.IsNullOrEmpty(res.answer))
                    {
                        // +++ เปลี่ยนจากการใส่ข้อความพรวดเดียว เป็นการเรียกแอนิเมชันพิมพ์ +++
                        StartCoroutine(TypeSentence(aiText, res.answer));
                    }
                    else
                    {
                        aiText.text = "<color=orange>Server response format error.</color>";
                        Debug.LogWarning("Server responded with valid JSON but empty answer.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("JSON Parse Error: " + ex.Message);
                    aiText.text = "<color=red>Data Error</color>"; 
                }
            }
        }

        sendButton.interactable = true;
    }

    IEnumerator TypeSentence(TMP_Text textComponent, string sentence)
    {
        textComponent.text = ""; 

        foreach (char letter in sentence.ToCharArray())
        {
            if (textComponent == null) yield break; 

            textComponent.text += letter;

            yield return new WaitForSeconds(typingSpeed); 
        }

        StartCoroutine(ForceScrollDown());
    }

    GameObject CreateBubble(GameObject prefab, string text)
    {
        GameObject newBubble = Instantiate(prefab, chatContent);
        TMP_Text bubbleText = newBubble.GetComponentInChildren<TMP_Text>();

        if (bubbleText != null)
        {
            bubbleText.text = text;
        }
        else
        {
            Debug.LogError("Bubble Prefab missing TMP_Text!");
        }
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
        
        StartCoroutine(ForceScrollDown());
    }

    void CloseChat()
    {
        chatWindow.SetActive(false);
        openButton.gameObject.SetActive(true);
    }
}