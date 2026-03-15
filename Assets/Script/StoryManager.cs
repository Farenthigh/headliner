using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // +++ 1. ต้องมีบรรทัดนี้เพื่อใช้ Coroutine +++
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

    // +++ 2. ตัวแปรใหม่สำหรับทำระบบพิมพ์ดีด +++
    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;  // ความเร็วในการพิมพ์ (ค่าน้อย = พิมพ์เร็ว)
    private bool isTyping = false;     // เช็คว่าตอนนี้กำลังพิมพ์อยู่หรือเปล่า
    private Coroutine typingCoroutine; // ตัวเก็บสถานะการพิมพ์

    void Start()
    {
        currentIndex = 0;
        UpdateUI();
    }

    public void OnClickNext()
    {
        // +++ 3. เช็คว่าถ้ากำลังพิมพ์อยู่ ให้ข้ามไปโชว์ข้อความเต็มๆ ทันที (ผู้เล่นใจร้อน) +++
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueTextUI.text = allPages[currentIndex].dialogueText;
            isTyping = false;
        }
        // แต่ถ้าพิมพ์เสร็จแล้ว ก็ให้เปลี่ยนไปหน้าถัดไปตามปกติ
        else
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
    }

    void UpdateUI()
    {
        StoryPage currentPage = allPages[currentIndex];

        // +++ 4. สั่งให้เริ่มพิมพ์ข้อความแทนการยัดข้อความใส่ตรงๆ +++
        if (dialogueTextUI != null) 
        {
            // ถ้ามีตัวเก่ากำลังพิมพ์อยู่ ให้หยุดก่อน
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            // สั่งเริ่มพิมพ์ประโยคของหน้าปัจจุบัน
            typingCoroutine = StartCoroutine(TypeSentence(currentPage.dialogueText));
        }

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

        // --- ส่วนของฉากจบ ---
        if(currentIndex == allPages.Count - 1)
        {
            Debug.Log("นี่คือหน้าสุดท้าย");
            Debug.Log("คุณชนะ");
            if (gameResult != null)
            {
                int stars = 0; // ใส่เกราะกัน Error (ดักไว้เผื่อหา Manager ไม่เจอ)
                if (Manager.Instance != null)
                {
                    stars = Manager.Instance.GetStarsFromExam();
                }
                else
                {
                    Debug.LogWarning("⚠️ หา Manager.Instance ไม่เจอ! จำลองดาว = 0");
                }
                
                gameResult.ShowVictoryResultDirect(stars);
            }
            
            if (nextButton != null) nextButton.SetActive(false);
        }
    }

    // +++ 5. ฟังก์ชันสำหรับทำเอฟเฟกต์พิมพ์ดีด +++
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueTextUI.text = ""; // ล้างหน้าจอให้ว่างเปล่าก่อน

        // เอาข้อความมาหั่นเป็นตัวอักษร แล้วค่อยๆ เติมเข้าไปทีละตัว
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed); // รอเวลาแป๊บนึงก่อนพิมพ์ตัวต่อไป
        }

        isTyping = false; // พิมพ์เสร็จสิ้น
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