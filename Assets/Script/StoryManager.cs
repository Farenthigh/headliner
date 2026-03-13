using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class StoryManager : MonoBehaviour
{
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

    [System.Serializable]
    public class TipBook
    {
        public GameObject tipBookUI;
        public int tipPageIndex;
    }

    [Header("Tip Books")]
    public List<TipBook> tipBooks;
    private bool isTipShowing = false;

    void Start()
    {
        currentIndex = 0;
        UpdateUI();
    }

    public void OnClickNext()
    {
        if (currentIndex < allPages.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
        else
        {
            Debug.Log("wไป Chapter ต่อไป");
        }
    }

    void UpdateUI()
    {
        StoryPage currentPage = allPages[currentIndex];

        if (dialogueTextUI != null) dialogueTextUI.text = currentPage.dialogueText;
        if (speakerNameUI != null) speakerNameUI.text = currentPage.speakerName;

        if (backgroundImageUI != null && currentPage.background != null)
        {
            backgroundImageUI.sprite = currentPage.background;
        }
       
        if (speechBubbleUI != null)
        {
            if (currentPage.speechBubble != null)
            {
                speechBubbleUI.gameObject.SetActive(true);
                speechBubbleUI.sprite = currentPage.speechBubble;
            }
            else
            {
                // ถ้าหน้าไหนไม่ใส่รูปกรอบคำพูดมา ให้ซ่อนกรอบไปเลย
                speechBubbleUI.gameObject.SetActive(false);
            }
        }
        HandleCharacterLayout(currentPage);

        // --- ส่วนของระบบ Quiz ---
        if (currentPage.isChoicePage)
        {
            if (nextButton != null) nextButton.SetActive(false);
            if (examSystem != null) 
            {
                examSystem.gameObject.SetActive(true);
                examSystem.StartExam();
            }
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
                    int stars = Manager.Instance.GetStarsFromExam();
                    gameResult.ShowVictoryResultDirect(stars);
                    // gameResult.ShowVictoryResultDirect(2); // ใส่จำนวนดาวที่ต้องการ
                }
                
                if (nextButton != null) nextButton.SetActive(false);
                // if (nextButton != null)
                //     nextButton.SetActive(false);
            }

                // รีเซ็ตก่อน
        isTipShowing = false;

        // ปิด tip ทุกอันก่อน
        foreach (TipBook tip in tipBooks)
        {
            if (tip.tipBookUI != null)
                tip.tipBookUI.SetActive(false);
        }

        // ตรวจว่า page นี้ต้องเปิด tip ไหม
        foreach (TipBook tip in tipBooks)
        {
            if (currentIndex == tip.tipPageIndex)
            {
                if (tip.tipBookUI != null)
                {
                    tip.tipBookUI.SetActive(true);
                    isTipShowing = true;
                }
            }
        }    
    }

    void HandleCharacterLayout(StoryPage page)
    {
        if (characterLeftUI != null) characterLeftUI.gameObject.SetActive(false);
        if (characterCenterUI != null) characterCenterUI.gameObject.SetActive(false);
        if (characterRightUI != null) characterRightUI.gameObject.SetActive(false);

        if (page.characterCenter != null && characterCenterUI != null)
        {
            SetupCharacter(characterCenterUI, page.characterCenter);
        }
        else
        {
            if (page.characterLeft != null && characterLeftUI != null)
            {
                SetupCharacter(characterLeftUI, page.characterLeft);
            }
            
            if (page.characterRight != null && characterRightUI != null)
            {
                SetupCharacter(characterRightUI, page.characterRight);
            }
        }
    }

    void SetupCharacter(Image characterUI, Sprite characterSprite)
    {
        characterUI.sprite = characterSprite;
        characterUI.gameObject.SetActive(true);
    }
}