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
        uint myUserId = (uint)APIManager.myData.id;
        string url = backendUrl + "/achievements/user/" + myUserId;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + APIManager.Token);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error โหลดข้อมูล: " + webRequest.error);
            }
            else
            {
                // ... (โค้ดดึง JSON ข้างในเหมือนเดิม ปล่อยไว้เลยครับ) ...
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
            ShowNotification(ach);

            StartCoroutine(SaveToBackendRoutine(backendAchievementId));
        }
    }

    IEnumerator SaveToBackendRoutine(uint achievementId)
    {
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

    // 1. ฟังก์ชันนี้เอาไว้ถูกเรียกตอนปลดล็อกสำเร็จ
    // 1. เปลี่ยนให้รับข้อมูลมาทั้งก้อน (AchievementData)
    public void ShowNotification(AchievementData data)
    {
        StartCoroutine(SlideNotificationRoutine(data));
    }

    // 2. ระบบแอนิเมชันสไลด์ขึ้น-ลง
    private System.Collections.IEnumerator SlideNotificationRoutine(AchievementData data)
    {
        // ใส่ชื่อถ้วยรางวัล
        if (popupNameText != null) popupNameText.text = data.achievementName;
        
        // +++ เพิ่มบรรทัดนี้ เพื่อเปลี่ยนรูปไอคอน! +++
        if (popupIconImage != null && data.icon != null) 
        {
            popupIconImage.sprite = data.icon;
        }
        
        // เปิดหน้าต่างขึ้นมา
        if (popupPanel != null) popupPanel.SetActive(true);

        // --- เตรียมทำแอนิเมชันสไลด์ ---
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

    [Header("Main UI")]
    public GameObject mainAchievementPanel; // ลาก MainAchievementPanel มาใส่ช่องนี้

    // สั่งเปิดหน้าต่างรวมถ้วยรางวัล
    public void OpenAchievementUI()
    {
        if (mainAchievementPanel != null)
        {
            GenerateAchievementUI(); // สั่งรีเฟรชข้อมูลให้ล่าสุดก่อนโชว์
            mainAchievementPanel.SetActive(true);
        }
    }

    // สั่งปิดหน้าต่าง
    public void CloseAchievementUI()
    {
        if (mainAchievementPanel != null)
        {
            mainAchievementPanel.SetActive(false);
        }
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