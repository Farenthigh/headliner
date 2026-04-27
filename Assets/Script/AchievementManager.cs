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

    [Header("New Unlock Animation UI")]
    public GameObject unlockPanel;
    public RectTransform cardRect;
    public AchievementCard achievementCard;
    public ParticleSystem sparkleParticle;

    [Header("Main UI")]
    public GameObject mainAchievementPanel;
    public GameObject achievementPrefab;
    public Transform achievementContainer;

    [Header("Hint Popup")]
    public GameObject hintPopup;
    public TextMeshProUGUI hintDescriptionText;

    // ✅ Callback: StoryManager หรือใครก็ตามที่ต้องรอให้การ์ดปิดก่อนดำเนินต่อ
    private System.Action _onUnlockPanelClosed;

    // ✅ ป้องกันการ unlock ซ้ำขณะ panel ยังเปิดอยู่
    private bool _isShowingUnlockPanel = false;

    // ✅ ให้ script อื่น (เช่น GameResult) เช็คได้ว่า panel ยังเปิดอยู่ไหม
    public bool IsShowingUnlockPanel => _isShowingUnlockPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (popupPanel        != null) popupPanel.SetActive(false);
        if (mainAchievementPanel != null) mainAchievementPanel.SetActive(false);
        if (unlockPanel       != null) unlockPanel.SetActive(false);
        if (cardRect          != null) cardRect.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(APIManager.Token))
            StartCoroutine(LoadDataFromBackendRoutine());
    }

    public void InitializeAfterLogin()
    {
        StartCoroutine(LoadDataFromBackendRoutine());
    }

    // ─── โหลดข้อมูลจาก Backend ───────────────────────────────────────────────
    IEnumerator LoadDataFromBackendRoutine()
    {
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
                AchievementListResponse response = JsonUtility.FromJson<AchievementListResponse>(jsonResponse);

                foreach (var ach in allAchievements)
                {
                    ach.isUnlocked = false;
                    ach.unlockDate = "";
                }

                if (response?.data != null)
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

    // ─── Unlock (ปกติ, ไม่มี callback) ──────────────────────────────────────
    public void UnlockAchievement(uint backendAchievementId, string localAchievementId)
    {
        UnlockAchievementInternal(backendAchievementId, localAchievementId, null);
    }

    // ✅ Unlock พร้อม callback: เรียกจาก StoryManager เพื่อรอให้การ์ดปิดก่อนดำเนินต่อ
    // ตัวอย่างการใช้งาน:
    //   AchievementManager.Instance.UnlockAchievementThenContinue(5, "5", () => {
    //       // โค้ดที่จะรันหลังผู้เล่นปิดการ์ด
    //       storyManager.NextLine();
    //   });
    public void UnlockAchievementThenContinue(uint backendAchievementId, string localAchievementId, System.Action onClosed)
    {
        UnlockAchievementInternal(backendAchievementId, localAchievementId, onClosed);
    }

    private void UnlockAchievementInternal(uint backendAchievementId, string localAchievementId, System.Action onClosed)
    {
        AchievementData ach = allAchievements.Find(a => a.id == localAchievementId);

        if (ach == null || ach.isUnlocked)
        {
            // Achievement นี้ปลดไปแล้ว → ไม่ต้องแสดงการ์ด แต่ยัง invoke callback ได้เลย
            onClosed?.Invoke();
            return;
        }

        ach.isUnlocked = true;
        ach.unlockDate = System.DateTime.Now.ToString("dd/MM/yyyy");
        Debug.Log($"ปลดล็อก: {ach.achievementName}");

        // ✅ เก็บ callback ไว้ CloseUnlockPanel จะเรียก
        _onUnlockPanelClosed = onClosed;

        StartCoroutine(PopupSequence(ach));
        StartCoroutine(SaveToBackendRoutine(backendAchievementId));
    }

    // ─── Animation เลื่อนการ์ดขึ้นมา ────────────────────────────────────────
    private IEnumerator PopupSequence(AchievementData data)
    {
        _isShowingUnlockPanel = true;

        achievementCard.SetupCard(data);
        unlockPanel.SetActive(true);
        cardRect.gameObject.SetActive(true);
        cardRect.localScale = Vector3.one;

        Vector2 startPos  = new Vector2(0, -800f);
        Vector2 targetPos = Vector2.zero;
        Vector3 startScale  = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 targetScale = Vector3.one;

        float duration = 0.5f, time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (time / duration), 3f); // easeOutCubic
            cardRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            cardRect.localScale       = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        cardRect.anchoredPosition = targetPos;
        cardRect.localScale       = targetScale;

        achievementCard.PlayUnlockVFX();
    }

    // ✅ ปิด Panel แล้วเรียก callback (StoryManager รอฟังตรงนี้)
    public void CloseUnlockPanel()
    {
        _isShowingUnlockPanel = false;
        unlockPanel.SetActive(false);
        cardRect.gameObject.SetActive(false);

        // invoke แล้วเคลียร์ทันที ป้องกันเรียกซ้ำ
        System.Action callback = _onUnlockPanelClosed;
        _onUnlockPanelClosed = null;
        callback?.Invoke();
    }

    // ─── Save to Backend ─────────────────────────────────────────────────────
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
            webRequest.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + APIManager.Token);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error บันทึก Achievement: " + webRequest.error);
            else
                Debug.Log("บันทึกลง Backend สำเร็จ User ID: " + myUserId);
        }
    }

    // ─── Popup (detail modal เมื่อกดจาก main panel) ──────────────────────────
    public void ShowPopup(AchievementData data)
    {
        if (popupPanel == null) return;
        popupNameText.text        = data.achievementName;
        popupDescriptionText.text = data.description;
        popupDateText.text        = data.unlockDate;
        popupIconImage.sprite     = data.icon;
        popupPanel.SetActive(true);
    }

    public void ClosePopup() => popupPanel.SetActive(false);

    // ─── Main Achievement UI ──────────────────────────────────────────────────
    public void OpenAchievementUI()
    {
        Debug.Log("OPEN ACHIEVEMENT UI");
        if (mainAchievementPanel != null)
        {
            GenerateAchievementUI();
            mainAchievementPanel.SetActive(true);
        }

        AchievementData ach = allAchievements.Find(a => a.id == "18");
        if (ach != null && !ach.isUnlocked)
            UnlockAchievement(18, "18");
    }

    public void CloseAchievementUI()
    {
        if (mainAchievementPanel != null)
            mainAchievementPanel.SetActive(false);
    }

    public void GenerateAchievementUI()
    {
        foreach (Transform child in achievementContainer)
            Destroy(child.gameObject);

        foreach (AchievementData ach in allAchievements)
        {
            GameObject newSlot = Instantiate(achievementPrefab, achievementContainer);
            AchievementSlotUI slotUI = newSlot.GetComponent<AchievementSlotUI>();
            slotUI.SetupSlot(ach);
        }
    }

    // ─── Hint Popup ───────────────────────────────────────────────────────────
    public void ShowHintPopup(AchievementData data)
    {
        hintPopup.SetActive(true);
        hintPopup.transform.SetAsLastSibling();
        hintDescriptionText.text = data.description;
    }

    public void CloseHintPopup() => hintPopup.SetActive(false);

    // ─── Achievement Checks ───────────────────────────────────────────────────
    public void CheckToolMasterGlobal()
    {
        bool usedChat = PlayerPrefs.GetInt("Used_Chat", 0) == 1;
        bool usedCalc = PlayerPrefs.GetInt("Used_Calculator", 0) == 1;
        bool usedTips = PlayerPrefs.GetInt("Used_Tips", 0) == 1;

        if (!usedChat || !usedCalc || !usedTips) return;

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

    public void CheckTaxComebackKing()
    {
        int level  = PlayerPrefs.GetInt("Tax_Level", 1);
        int failed = PlayerPrefs.GetInt("Tax_Failed", 0);

        if (level > 5 && failed == 0)
        {
            AchievementData ach = allAchievements.Find(a => a.id == "23");
            if (ach != null && !ach.isUnlocked)
                UnlockAchievement(23, "23");
        }
    }

    public void CheckDoubleExpertise()
    {
        bool playedTax    = PlayerPrefs.GetInt("Played_Tax", 0) == 1;
        bool playedSaving = PlayerPrefs.GetInt("Played_Saving", 0) == 1;

        if (!playedTax || !playedSaving) return;

        AchievementData ach = allAchievements.Find(a => a.id == "25");
        if (ach != null && !ach.isUnlocked)
        {
            UnlockAchievement(25, "25");
            PlayerPrefs.DeleteKey("Played_Tax");
            PlayerPrefs.DeleteKey("Played_Saving");
            PlayerPrefs.Save();
        }
    }
}

// ─── Data Classes ─────────────────────────────────────────────────────────────
[System.Serializable]
public class UnlockRequest
{
    public uint user_id;
    public uint achievement_id;
}

[System.Serializable]
public class UserAchievementResponse
{
    public uint   user_id;
    public uint   achievement_id;
    public string unlocked_at;
}

[System.Serializable]
public class AchievementListResponse
{
    public string message;
    public List<UserAchievementResponse> data;
}