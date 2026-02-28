using UnityEngine;
using UnityEngine.Networking; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    public string backendUrl = "http://localhost:8080"; 
    public uint currentUserId = 1; 
    public List<AchievementData> allAchievements;
    public GameObject popupPanel;         
    public TextMeshProUGUI popupNameText;             
    public TextMeshProUGUI popupDescriptionText;      
    public TextMeshProUGUI popupDateText;             
    public Image popupIconImage;
    public GameObject achievementPrefab;   
    public Transform achievementContainer;


    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);

        StartCoroutine(LoadDataFromBackendRoutine());
    }

    IEnumerator LoadDataFromBackendRoutine()
    {
        string url = backendUrl + "/achievements/user/" + currentUserId;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error โหลดข้อมูล: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                
                AchievementListResponse response = JsonUtility.FromJson<AchievementListResponse>(jsonResponse);

                foreach (var ach in allAchievements) 
                {
                    ach.isUnlocked = false; 
                    ach.unlockDate = "";
                }

                if (response != null && response.data != null)
                {
                    foreach (var backendData in response.data)
                    {
                        AchievementData ach = allAchievements.Find(a => a.id == backendData.achievement_id.ToString());
                        if (ach != null)
                        {
                            ach.isUnlocked = true;
                            ach.unlockDate = backendData.unlocked_at.Split('T')[0]; 
                        }
                    }
                }
                GenerateAchievementUI();
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

    public void GenerateAchievementUI()
    {
        foreach (Transform child in achievementContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (AchievementData ach in allAchievements)
        {
            GameObject newSlot = Instantiate(achievementPrefab, achievementContainer);
            
            AchievementSlotUI slotUI = newSlot.GetComponent<AchievementSlotUI>();
            slotUI.SetupSlot(ach);
        }
    }

    [ContextMenu("Test Unlock to Real DB!")]
    public void TestUnlockToDB()
    {
        UnlockAchievement(1, "1"); 
        UnlockAchievement(2, "2"); 

        GenerateAchievementUI(); 
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