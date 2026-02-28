using UnityEngine;
using UnityEngine.Networking; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    public string backendUrl = "http://localhost:8080"; 
    public uint currentUserId = 1; 
    public List<AchievementData> allAchievements;
    public GameObject popupPanel;         
    public Text popupNameText;             
    public Text popupDescriptionText;      
    public Text popupDateText;             
    public Image popupIconImage;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        StartCoroutine(LoadDataFromBackendRoutine());
    }

    IEnumerator LoadDataFromBackendRoutine()
    {
        string url = backendUrl + "/achievements/user/" + currentUserId;
        Debug.Log("กำลังโหลดข้อมูลจาก: " + url);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error โหลดข้อมูล: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("ได้ข้อมูล: " + jsonResponse);
            }
        }
    }
    public void UnlockAchievement(uint backendAchievementId, string localAchievementId)
    {
        AchievementData ach = allAchievements.Find(a => a.id == localAchievementId);
        if (ach != null && !ach.isUnlocked)
        {
            ach.isUnlocked = true;
            ach.unlockDate = System.DateTime.Now.ToString("dd/MM/yyyy");
            Debug.Log($"ปลดล็อก UI: {ach.achievementName}!");

            StartCoroutine(SaveToBackendRoutine(backendAchievementId));
        }
    }

    IEnumerator SaveToBackendRoutine(uint achievementId)
    {
        string url = backendUrl + "/achievements/unlock";

        UnlockRequest req = new UnlockRequest { user_id = currentUserId, achievement_id = achievementId };
        string jsonData = JsonUtility.ToJson(req);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error บันทึก Achievement: " + webRequest.error);
            }
            else
            {
                Debug.Log("บันทึกลง Backend");
            }
        }
    }

    public void ShowPopup(AchievementData data)
    {
        popupNameText.text = data.achievementName;
        popupDescriptionText.text = "Description: " + data.description;
        popupDateText.text = "Date: " + data.unlockDate;
        popupIconImage.sprite = data.icon;

        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}

[System.Serializable]
public class UnlockRequest
{
    public uint user_id;
    public uint achievement_id;
}

[System.Serializable]
public class UserAchievementResponse
{
    public uint user_id;
    public uint achievement_id;
    public string unlocked_at;
}

[System.Serializable]
public class AchievementListResponse
{
    public string message;
    public List<UserAchievementResponse> data; 
}