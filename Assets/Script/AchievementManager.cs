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
    public List<AchievementData> allAchievements;
    
    [Header("Popup UI")]
    public GameObject popupPanel;         
    public TextMeshProUGUI popupNameText;             
    public TextMeshProUGUI popupDescriptionText;      
    public TextMeshProUGUI popupDateText;             
    public Image popupIconImage;

    [Header("Main UI")]
    public GameObject mainAchievementPanel; 
    public GameObject achievementPrefab;   
    public Transform achievementContainer;
    [Header("Hint Popup")]
    public GameObject hintPopup;
    public TextMeshProUGUI hintDescriptionText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
        if (mainAchievementPanel != null) mainAchievementPanel.SetActive(false);

        if (!string.IsNullOrEmpty(APIManager.Token))
        {
            StartCoroutine(LoadDataFromBackendRoutine());
        }
        

    }

    public void InitializeAfterLogin()
    {
        StartCoroutine(LoadDataFromBackendRoutine());
    }

    IEnumerator LoadDataFromBackendRoutine()
    {
        // แก้ไขการดัก Error: เปลี่ยนมาเช็ค Token แทนเพราะ UserData เป็น struct เช็ค null ไม่ได้
        if (string.IsNullOrEmpty(APIManager.Token)) yield break;

        uint myUserId = (uint)APIManager.myData.id;
        string url = backendUrl + "/achievements/user/" + myUserId;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + APIManager.Token);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error โหลดข้อมูล: " + webRequest.error);
                GenerateAchievementUI(); 
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("JSON จาก Backend: " + jsonResponse);
                AchievementListResponse response = JsonUtility.FromJson<AchievementListResponse>(jsonResponse);
                Debug.Log("ALL ACH COUNT: " + allAchievements.Count);
                foreach (var ach in allAchievements) 
                {
                    ach.isUnlocked = false; 
                    ach.unlockDate = "";
                }

                if (response != null && response.data != null)
                {
                    foreach (var backendData in response.data)
                    {
                        if (backendData.user_id != myUserId) continue; 
                        
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
            ShowNotification(ach);

            StartCoroutine(SaveToBackendRoutine(backendAchievementId));
        }
    }

    IEnumerator SaveToBackendRoutine(uint achievementId)
    {
        if (string.IsNullOrEmpty(APIManager.Token)) yield break;

        string url = backendUrl + "/achievements/unlock";
        uint myUserId = (uint)APIManager.myData.id;
        UnlockRequest req = new UnlockRequest { user_id = myUserId, achievement_id = achievementId };
        string jsonData = JsonUtility.ToJson(req);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + APIManager.Token);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error บันทึก Achievement: " + webRequest.error);
            }
            else
            {
                Debug.Log("บันทึกลง Backend สำเร็จ ของ User ID: " + myUserId);
            }
        }
    }

    public void ShowNotification(AchievementData data)
    {
        StartCoroutine(SlideNotificationRoutine(data));
    }

    private System.Collections.IEnumerator SlideNotificationRoutine(AchievementData data)
    {
        if (popupNameText != null) popupNameText.text = data.achievementName;
        if (popupIconImage != null && data.icon != null) popupIconImage.sprite = data.icon;
        if (popupPanel != null) popupPanel.SetActive(true);

        RectTransform rect = popupPanel.GetComponent<RectTransform>();
        Vector2 hiddenPos = new Vector2(rect.anchoredPosition.x, -150f); 
        Vector2 showPos = new Vector2(rect.anchoredPosition.x, 20f);

        rect.anchoredPosition = hiddenPos;

        float time = 0;
        while(time < 0.5f) 
        {
            rect.anchoredPosition = Vector2.Lerp(hiddenPos, showPos, time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = showPos;

        yield return new WaitForSeconds(3f);

        time = 0;
        while(time < 0.5f) 
        {
            rect.anchoredPosition = Vector2.Lerp(showPos, hiddenPos, time / 0.5f);
            time += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = hiddenPos;

        popupPanel.SetActive(false);
    }

    // --- ส่วนที่ผมเผลอลบไป เอาคืนมาแล้วครับ! ---
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

    public void OpenAchievementUI()
    {   Debug.Log("OPEN ACHIEVEMENT UI"); 
        if (mainAchievementPanel != null)
        {
            GenerateAchievementUI(); 
            mainAchievementPanel.SetActive(true);
        }

        AchievementData ach = allAchievements.Find(a => a.id == "18");
        if (ach != null && !ach.isUnlocked)
        {
            AchievementManager.Instance.UnlockAchievement(18, "18");
        }
    }

    public void CloseAchievementUI()
    {
        if (mainAchievementPanel != null)
        {
            mainAchievementPanel.SetActive(false);
        }
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

    public void CheckToolMasterGlobal()
    {
        bool usedChat = PlayerPrefs.GetInt("Used_Chat", 0) == 1;
        bool usedCalc = PlayerPrefs.GetInt("Used_Calculator", 0) == 1;
        bool usedTips = PlayerPrefs.GetInt("Used_Tips", 0) == 1;

        if (usedChat && usedCalc && usedTips)
        {
            AchievementData ach = allAchievements.Find(a => a.id == "22");

            if (ach != null && !ach.isUnlocked)
            {
                UnlockAchievement(22, "22");

                PlayerPrefs.DeleteKey("Used_Chat");
                PlayerPrefs.DeleteKey("Used_Calculator");
                PlayerPrefs.DeleteKey("Used_Tips");
                PlayerPrefs.Save();
            }
        }
    }

    public void CheckTaxComebackKing()
    {
        int level = PlayerPrefs.GetInt("Tax_Level", 1);
        int failed = PlayerPrefs.GetInt("Tax_Failed", 0);

        if (level > 5 && failed == 0)
        {
            AchievementData ach = allAchievements.Find(a => a.id == "23");

            if (ach != null && !ach.isUnlocked)
            {
                UnlockAchievement(23, "23");
            }
        }
    }
    public void CheckDoubleExpertise()
    {
        bool playedTax = PlayerPrefs.GetInt("Played_Tax", 0) == 1;
        bool playedSaving = PlayerPrefs.GetInt("Played_Saving", 0) == 1;

        if (playedTax && playedSaving)
        {
            AchievementData ach = allAchievements.Find(a => a.id == "25");

            if (ach != null && !ach.isUnlocked)
            {
                UnlockAchievement(25, "25");

            // reset กัน spam
                PlayerPrefs.DeleteKey("Played_Tax");
                PlayerPrefs.DeleteKey("Played_Saving");
                PlayerPrefs.Save();
            }
        }
    }
   public void ShowHintPopup(AchievementData data)
    {
        Debug.Log("Show Hint Popup: " + data.name);

        hintPopup.SetActive(true);

        Debug.Log("Popup Active: " + hintPopup.activeSelf);
        Debug.Log("Popup In Hierarchy: " + hintPopup.activeInHierarchy);

        hintPopup.transform.SetAsLastSibling();

        hintDescriptionText.text = data.description;
    }   
    public void CloseHintPopup()
    {
        Debug.Log("CLOSE POPUP");
        hintPopup.SetActive(false);
    }
}

// --- คลาสสำหรับใช้แกะ JSON ที่หายไป กลับมาแล้วครับ! ---
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

