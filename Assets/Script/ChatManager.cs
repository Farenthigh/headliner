using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; 

public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text chatHistoryText;      
    public TMP_InputField messageInput;   
    public UnityEngine.UI.Button sendButton; 

    private string apiUrl = "http://localhost:8080/api/chat";

    [System.Serializable]
    public class ChatRequest
    {
        public string player_id;
        public string message;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public string answer;
        public string status;
    }

    void Start()
    {
        sendButton.onClick.AddListener(SendMessageToAI);
        
        chatHistoryText.text = "--- เริ่มต้นการสนทนา ---";
    }

    public void SendMessageToAI()
    {
        if (string.IsNullOrEmpty(messageInput.text)) return;

        string userMessage = messageInput.text;
        
        // แสดงข้อความผู้เล่นบนหน้าจอทันที
        UpdateChatDisplay($"<color=#00FFFF><b>Player:</b></color> {userMessage}"); // สีฟ้า
        
        // เคลียร์ช่องพิมพ์
        messageInput.text = "";

        // ริ่มส่งข้อมูล (Coroutine)
        StartCoroutine(PostRequest(userMessage));
    }

    IEnumerator PostRequest(string message)
    {
        // เตรียมข้อมูล JSON
        ChatRequest reqData = new ChatRequest
        {
            player_id = "Tester001", 
            message = message
        };
        
        string json = JsonUtility.ToJson(reqData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            UpdateChatDisplay("<i><color=grey>AI กำลังคิด...</color></i>");

            // ส่งและรอผลลัพธ์
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                UpdateChatDisplay($"<color=red><b>Error:</b> {request.error}</color>");
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(responseText);
                
                UpdateChatDisplay($"<color=#00FF00><b>AI:</b></color> {response.answer}");
            }
        }
    }
    void UpdateChatDisplay(string newMessage)
    {
        chatHistoryText.text += "\n\n" + newMessage;
    }
}