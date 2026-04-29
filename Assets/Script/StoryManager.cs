using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using UnityEngine.Video;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [Header("Controllers")]
    public TransitionController transitionController; // <--- นำกลับมาจาก Current

    [Header("Story UI")]
    public Text dialogueTextUI;
    public Text speakerNameUI;
    public Image backgroundImageUI;
    public Image characterLeftUI;
    public Image characterCenterUI;
    public Image characterRightUI;
    public Image speechBubbleUI;
    public GameObject nextButton;
    public List<StoryPage> allPages;
    private int currentIndex = 0;
    public GameResult gameResult;

    [Header("Quiz UI")]
    public Examlogic examSystem;

    [Header("Stage Info")]
    [SerializeField] private int currentStage = 1;

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;  
    private bool isTyping = false;     
    private Coroutine typingCoroutine; 

    // +++ ส่วนของ VS Animation +++
    [Header("VS Animation Settings (ลากของมาใส่)")]
    public GameObject vsPanel;
    public RectTransform vTransform;
    public RectTransform sTransform;
    public RectTransform topCloud;     
    public RectTransform bottomCloud;  
    public ParticleSystem clashParticle; 
    public AudioSource thunderSound;   
    public float animationDuration = 0.5f;

    [Header("Target Positions (จุดที่มันจะวิ่งมาหยุด)")]
    public Vector2 vTargetPos = new Vector2(-19f, 49f); 
    public Vector2 sTargetPos = new Vector2(72f, -37f);  
    public Vector2 topCloudTargetPos = new Vector2(0f, 407.92f);    
    public Vector2 bottomCloudTargetPos = new Vector2(0f, -407.92f); 

    [Header("Naming System")]
    public GameObject namingPanel;
    public TMP_InputField nameInputField;
    public Button confirmNameButton;
    private string currentChatbotName = "Chatbot";

    [Header("Item Page")]
    public GameObject itemPanel;
    public Image itemSpriteUI;    // Image ใน itemPanel
    public Sprite itemSprite;     // รูปไอเทม (ใส่ใน Inspector ครั้งเดียว)
    public TMP_Text itemNameText;
    public TMP_Text itemSubNameText;

    [System.Serializable]
    public class TipBook
    {
        public int tipPageIndex;      
        public int bookPageToOpen;  
    }

    [Header("Tip Books")]
    public List<TipBook> tipBooks;
    public SceneLoader sceneLoader;

        [System.Serializable]
    public class StoryAchievementTrigger
    {
        public int pageIndex;          // หน้าไหนจะ unlock
        public uint backendAchievementId;
        public string localAchievementId;
    }

    [Header("Achievement Triggers")]
    public List<StoryAchievementTrigger> achievementTriggers;

    // ✅ ป้องกันกดปุ่ม Next ขณะ Achievement panel กำลังแสดงอยู่
    private bool _isWaitingForAchievement = false;

    [Header("Video Background")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;

    [Header("Player Characters")]
    public Sprite[] availableCharacters;

    void Start()
    {
        if(vsPanel != null) vsPanel.SetActive(false);
        if(clashParticle != null) 
        {
            clashParticle.Stop();
            clashParticle.Clear();
        }
        if (videoPlayer != null && videoRawImage != null)
        {
            videoRawImage.texture = videoPlayer.targetTexture;
        }

        // ตั้งค่าปุ่ม Confirm Name
        if (confirmNameButton != null)
        {
            confirmNameButton.onClick.AddListener(ConfirmName);
        }
        if (itemPanel != null) itemPanel.SetActive(false);

        // เช็คชื่อเก่า
        if (PlayerPrefs.HasKey("ChatbotCustomName"))
        {
            currentChatbotName = PlayerPrefs.GetString("ChatbotCustomName");
        }
        else if (APIManager.myData.id != 0 && !string.IsNullOrEmpty(APIManager.myData.chatbot_name))
        {
            currentChatbotName = APIManager.myData.chatbot_name;
        }

        currentIndex = 0;
        UpdateUI();
    }

    public void OnClickNext()
    {
        // ✅ ถ้า Achievement panel กำลังเปิดอยู่ → ไม่ให้กดผ่าน
        if (_isWaitingForAchievement) return;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueTextUI.text = ProcessText(allPages[currentIndex].dialogueText); 
            isTyping = false;
        }
        else
        {
            if (currentIndex < allPages.Count - 1)
            {
                AdvanceToPage(currentIndex + 1);
            }
            else
            {
                Debug.Log("ไป Chapter ต่อไป");
            }
        }
    }

    // ✅ แยก logic advance ออกมาเป็น method ให้ทั้ง OnClickNext และ achievement callback เรียกได้
    private void AdvanceToPage(int nextIndex)
    {
        // ✅ เช็ค Achievement trigger ก่อนเปลี่ยนหน้า
        //    ถ้ามี trigger ที่ pageIndex == nextIndex → แสดงการ์ดก่อน แล้วค่อยเปลี่ยนหน้าใน callback
        StoryAchievementTrigger trigger = achievementTriggers?.Find(t => t.pageIndex == nextIndex);
        if (trigger != null && AchievementManager.Instance != null)
        {
            _isWaitingForAchievement = true;
            SetNextButtonInteractable(false);

            AchievementManager.Instance.UnlockAchievementThenContinue(
                trigger.backendAchievementId,
                trigger.localAchievementId,
                onClosed: () =>
                {
                    _isWaitingForAchievement = false;
                    SetNextButtonInteractable(true);
                    DoPageTransition(nextIndex);
                }
            );
            return;
        }

        DoPageTransition(nextIndex);
    }

    // ✅ helper แยก transition ออกมาให้ AdvanceToPage และ achievement callback เรียกได้เหมือนกัน
    private void DoPageTransition(int nextIndex)
    {
        Sprite currentBg = allPages[currentIndex].background;
        Sprite nextBg    = allPages[nextIndex].background;
        bool isBgChanged  = (currentBg != nextBg && nextBg != null);
        bool isForcedFade = allPages[nextIndex].forceTransition;

        if ((isBgChanged || isForcedFade) && transitionController != null)
        {
            SetNextButtonInteractable(false);

            transitionController.PlaySceneFade(
                onMidFade: () => {
                    currentIndex = nextIndex;
                    UpdateUI();
                },
                onComplete: () => {
                    SetNextButtonInteractable(true);
                }
            );
        }
        else
        {
            currentIndex = nextIndex;
            UpdateUI();
        }
    }

    // ✅ helper ตั้ง interactable ปุ่ม Next อย่างปลอดภัย
    private void SetNextButtonInteractable(bool interactable)
    {
        if (nextButton == null) return;
        Button btn = nextButton.GetComponent<Button>();
        if (btn != null) btn.interactable = interactable;
    }

    public void SetOnlyEnemySpeaking()
    {
        StartMove(characterRightUI);
        StopMove(characterLeftUI);

        if (characterRightUI != null) characterRightUI.color = new Color(1, 1, 1, 1f);
        if (characterLeftUI != null) characterLeftUI.color = new Color(1, 1, 1, 1f);
    }

    // ฟังก์ชันแปลงแท็ก {Name} ให้กลายเป็นชื่อที่ผู้เล่นตั้ง
    private string ProcessText(string rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return "";
        return rawText.Replace("{Name}", currentChatbotName);
    }

    void UpdateUI()
    {
        StoryPage currentPage = allPages[currentIndex];

        if (currentPage.isNamingPage)
        {
            if (namingPanel != null) namingPanel.SetActive(true);
            if (nextButton != null) nextButton.SetActive(false);
            return; 
        }

        // ✅ Item Page
        if (currentPage.isItemPage)
        {
            if (itemPanel != null)
            {
                itemPanel.SetActive(true);
                if (itemSpriteUI != null && itemSprite != null)
                    itemSpriteUI.sprite = itemSprite;
                if (itemNameText != null)
                    itemNameText.text = currentPage.itemName;
                if (itemSubNameText != null)
                    itemSubNameText.text = currentPage.itemSubName;
            }
            if (nextButton != null) nextButton.SetActive(true);
            return;
        }

        // ออกจาก item page → เปิด story UI กลับมา
        if (dialogueTextUI != null) dialogueTextUI.gameObject.SetActive(true);
        if (speakerNameUI != null) speakerNameUI.gameObject.SetActive(true);
        if (itemPanel != null) itemPanel.SetActive(false);

        if (namingPanel != null) namingPanel.SetActive(false);
        if (nextButton != null) nextButton.SetActive(true);

        string processedSpeaker = ProcessText(currentPage.speakerName);
        string processedDialogue = ProcessText(currentPage.dialogueText);

        if (dialogueTextUI != null) 
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence(processedDialogue));
        }

        if (speakerNameUI != null) speakerNameUI.text = currentPage.speakerName;

        // รีเซ็ตก่อนทุกครั้ง
        if (backgroundImageUI != null)
            backgroundImageUI.gameObject.SetActive(false);

        if (videoRawImage != null)
            videoRawImage.gameObject.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
        }

        // ถ้ามีวิดีโอ ให้ใช้วิดีโอก่อน
        if (currentPage.videoBackground != null)
        {
            Debug.Log("VIDEO PAGE => " + currentIndex + " / " + currentPage.videoBackground.name);

            if (videoPlayer != null)
            {
                videoPlayer.clip = currentPage.videoBackground;
                videoPlayer.isLooping = true;
                videoPlayer.Prepare();
                videoPlayer.Play();
            }

            if (videoRawImage != null && videoPlayer != null)
            {
                videoRawImage.texture = videoPlayer.targetTexture;
                videoRawImage.gameObject.SetActive(true);
            }
        }
        // ถ้าไม่มีวิดีโอ แต่มีรูป ให้ใช้รูป
        else if (currentPage.background != null)
        {
            Debug.Log("IMAGE PAGE => " + currentIndex + " / " + currentPage.background.name);

            if (backgroundImageUI != null)
            {
                backgroundImageUI.sprite = currentPage.background;
                backgroundImageUI.gameObject.SetActive(true);
            }
        }
        
        
        if (speakerNameUI != null) speakerNameUI.text = processedSpeaker;
        if (backgroundImageUI != null && currentPage.background != null) backgroundImageUI.sprite = currentPage.background;
       
        if (speechBubbleUI != null)
        {
            if (currentPage.speechBubble != null)
            {
                speechBubbleUI.gameObject.SetActive(true);
                speechBubbleUI.sprite = currentPage.speechBubble;
            }
            else speechBubbleUI.gameObject.SetActive(false);
        }
        
        HandleCharacterLayout(currentPage);

        if (!currentPage.isChoicePage)
        {
            UpdateCharacterAnimation(currentPage);
        }

        if (currentPage.isChoicePage)
        {
            if (nextButton != null) nextButton.SetActive(false);
            StartCoroutine(PlayVSEffectAndStartQuiz());
        }
        else
        {
            if (nextButton != null) nextButton.SetActive(true);
            if (examSystem != null) examSystem.gameObject.SetActive(false);
        }

        if(currentIndex == allPages.Count - 1)
        {
            Debug.Log("นี่คือหน้าสุดท้าย");
            Debug.Log("คุณชนะ");

            if (gameResult != null)
            {
                int stars = 0; 
                if (Manager.Instance != null) 
                {
                    stars = Manager.Instance.GetStarsFromExam();
                }
                Debug.Log("⭐ จำนวนดาวที่จะส่งไปลีดเดอร์บอร์ดคือ: " + stars);
                // ใช้การเรียกฟังก์ชันแบบ Incoming
                gameResult.ShowVictoryResultDirect(stars, currentStage, stars, 0); 
            }

            if (nextButton != null) nextButton.SetActive(false);
        }

        if (sceneLoader != null)
            sceneLoader.ExitTips();

        int bestPage = 0;
        int bestIndex = -1;

        foreach (TipBook tip in tipBooks)
        {
            if (currentIndex >= tip.tipPageIndex && tip.tipPageIndex > bestIndex)
            {
                bestIndex = tip.tipPageIndex;
                bestPage = tip.bookPageToOpen;
            }
        }

        if (sceneLoader != null)
            sceneLoader.defaultPageToOpen = bestPage;

        // เปิดอัตโนมัติถ้าตรงกับฉากนี้พอดี
        foreach (TipBook tip in tipBooks)
        {
            if (currentIndex == tip.tipPageIndex)
            {
                if (sceneLoader != null)
                    sceneLoader.OpenTips();
            }
        }
    }
    
    IEnumerator PlayVSEffectAndStartQuiz()
    {

        if (characterLeftUI != null) characterLeftUI.gameObject.SetActive(false);
        if (characterCenterUI != null) characterCenterUI.gameObject.SetActive(false);
        if (characterRightUI != null) characterRightUI.gameObject.SetActive(false);

        if(vsPanel != null) vsPanel.SetActive(true);
        if(clashParticle != null) 
        {
            clashParticle.Stop();
            clashParticle.Clear();
        }

        Vector2 vStartPos = vTransform != null ? vTransform.anchoredPosition : Vector2.zero;
        Vector2 sStartPos = sTransform != null ? sTransform.anchoredPosition : Vector2.zero;
        Vector2 topCloudStartPos = topCloud != null ? topCloud.anchoredPosition : Vector2.zero;
        Vector2 bottomCloudStartPos = bottomCloud != null ? bottomCloud.anchoredPosition : Vector2.zero;

        float time = 0;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float percent = time / animationDuration;

            if (vTransform != null) vTransform.anchoredPosition = Vector2.Lerp(vStartPos, vTargetPos, percent);
            if (sTransform != null) sTransform.anchoredPosition = Vector2.Lerp(sStartPos, sTargetPos, percent);
            if (topCloud != null) topCloud.anchoredPosition = Vector2.Lerp(topCloudStartPos, topCloudTargetPos, percent);
            if (bottomCloud != null) bottomCloud.anchoredPosition = Vector2.Lerp(bottomCloudStartPos, bottomCloudTargetPos, percent);
            
            yield return null;
        }

        if (vTransform != null) vTransform.anchoredPosition = vTargetPos;
        if (sTransform != null) sTransform.anchoredPosition = sTargetPos;
        if (topCloud != null) topCloud.anchoredPosition = topCloudTargetPos;
        if (bottomCloud != null) bottomCloud.anchoredPosition = bottomCloudTargetPos;

        if (clashParticle != null) clashParticle.Play();
        if (thunderSound != null) thunderSound.Play(); 

        yield return new WaitForSeconds(3f); 

        if(vsPanel != null) vsPanel.SetActive(false);
        if (vTransform != null) vTransform.anchoredPosition = vStartPos;
        if (sTransform != null) sTransform.anchoredPosition = sStartPos;
        if (topCloud != null) topCloud.anchoredPosition = topCloudStartPos;
        if (bottomCloud != null) bottomCloud.anchoredPosition = bottomCloudStartPos;

        if(clashParticle != null) clashParticle.Stop();

        ForceUpdateExamCharacter();

        if (examSystem != null) 
        {
            examSystem.gameObject.SetActive(true);
            examSystem.StartExam();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueTextUI.text = ""; 
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed); 
        }
        isTyping = false; 
    }
    
    void HandleCharacterLayout(StoryPage page)
    {
        if (characterLeftUI != null) characterLeftUI.gameObject.SetActive(false);
        if (characterCenterUI != null) characterCenterUI.gameObject.SetActive(false);
        if (characterRightUI != null) characterRightUI.gameObject.SetActive(false);

        Sprite centerSprite = page.characterCenter;
        Sprite leftSprite = page.characterLeft;
        Sprite rightSprite = page.characterRight;

        if (page.isPlayer && availableCharacters.Length > 0)
        {
            int selectedCharID = 0; 
            
            if (APIManager.myData.id != 0) 
            {
                selectedCharID = APIManager.myData.character; 
            }
            else 
            {
                selectedCharID = PlayerPrefs.GetInt("SelectedCharacter", 0); 
            }

            if (selectedCharID >= 0 && selectedCharID < availableCharacters.Length)
            {
                Sprite playerSprite = availableCharacters[selectedCharID];

                switch (page.speakerPosition)
                {
                    case SpeakerPosition.Left: leftSprite = playerSprite; break;
                    case SpeakerPosition.Center: centerSprite = playerSprite; break;
                    case SpeakerPosition.Right: rightSprite = playerSprite; break;
                }
            }
        }

        // ส่งตัวแปร page.forceCharacterFade (จาก Current) เข้าไปใช้ใน SetupCharacter ด้วย
        if (centerSprite != null && characterCenterUI != null) 
            SetupCharacter(characterCenterUI, centerSprite, page.forceCharacterFade);
        else
        {
            if (leftSprite != null && characterLeftUI != null) 
                SetupCharacter(characterLeftUI, leftSprite, page.forceCharacterFade);
            if (rightSprite != null && characterRightUI != null) 
                SetupCharacter(characterRightUI, rightSprite, page.forceCharacterFade);
        }
    }

    // อัปเดตให้รองรับ forceFade (นำกลับมาจาก Current)
    void SetupCharacter(Image characterUI, Sprite characterSprite, bool forceFade)
    {
        characterUI.sprite = characterSprite;
        characterUI.gameObject.SetActive(true);

        if (forceFade && transitionController != null)
        {
            transitionController.FadeInCharacter(characterUI);
        }
        else
        {
            Color c = characterUI.color;
            c.a = 1f;
            characterUI.color = c;
        }
    }

    void StartMove(Image character)
    {
        if (character == null) return;

        CharacterBounce bounce = character.GetComponent<CharacterBounce>();
        if (bounce == null)
        {
            bounce = character.gameObject.AddComponent<CharacterBounce>();
            // ลบบรรทัดที่ AddComponent ซ้ำกันออกให้แล้วครับ
        }

        bounce.enabled = true; 
    }  

    void StopMove(Image character)
    {
        if (character == null) return;

        CharacterBounce bounce = character.GetComponent<CharacterBounce>();
        if (bounce != null)
        {
            bounce.enabled = false; 
        }
    }

    void UpdateCharacterAnimation(StoryPage page)
    {
        Debug.Log("Animating: " + page.speakerPosition);
        
        StopMove(characterLeftUI);
        StopMove(characterCenterUI);
        StopMove(characterRightUI);

        ResetPosition(characterLeftUI);
        ResetPosition(characterCenterUI);
        ResetPosition(characterRightUI);

        switch (page.speakerPosition)
        {
            case SpeakerPosition.Left:
                StartMove(characterLeftUI);
                break;
            case SpeakerPosition.Center:
                StartMove(characterCenterUI);
                break;
            case SpeakerPosition.Right:
                StartMove(characterRightUI);
                break;
        }
    }

    void ResetPosition(Image character)
    {
        if (character == null) return;

        RectTransform rect = character.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
        }
    }

    private async void SendResult(int stars)
    {
        Debug.Log("Sending stars: " + stars + " stage: " + currentStage);
        await APIManager.Instance.SaveGameResult(stars, currentStage);
    }
    

    // ฟังก์ชันกดยืนยันตั้งชื่อ (จาก Incoming)
    public async void ConfirmName()
    {
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            string newName = nameInputField.text;
            currentChatbotName = newName;
            await APIManager.Instance.SaveChatbotName(newName);
        }
        if (namingPanel != null) namingPanel.SetActive(false);
        if (currentIndex < allPages.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void ForceUpdateExamCharacter()
    {
        StoryPage currentPage = allPages[currentIndex];

        int selectedCharID = 0;
        if (APIManager.myData.id != 0) {
            selectedCharID = APIManager.myData.character;
        } else {
            selectedCharID = PlayerPrefs.GetInt("SelectedCharacter", 0);
        }

        if (availableCharacters != null && selectedCharID < availableCharacters.Length) {
            Sprite playerSprite = availableCharacters[selectedCharID];
            if (characterLeftUI != null) {
                characterLeftUI.sprite = playerSprite;
                characterLeftUI.gameObject.SetActive(true);
                
                Color c = characterLeftUI.color;
                c.a = 1f;
                characterLeftUI.color = c;
            }
        }

        if (currentPage.characterRight != null && characterRightUI != null) {
            characterRightUI.sprite = currentPage.characterRight;
            characterRightUI.gameObject.SetActive(true);

            Color c = characterRightUI.color;
            c.a = 1f;
            characterRightUI.color = c;
        }
    }
}