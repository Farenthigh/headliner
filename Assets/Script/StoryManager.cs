using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
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

    void Start()
    {
        if(vsPanel != null) vsPanel.SetActive(false);
        if(clashParticle != null) 
        {
            clashParticle.Stop();
            clashParticle.Clear();
        }
        currentIndex = 0;
        UpdateUI();
    }

    public void OnClickNext()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueTextUI.text = allPages[currentIndex].dialogueText;
            isTyping = false;
        }
        else
        {
            if (currentIndex < allPages.Count - 1)
            {
                currentIndex++;
                UpdateUI();
            }
            else
            {
                Debug.Log("ไป Chapter ต่อไป");
            }
        }
    }

    public void SetOnlyEnemySpeaking()
    {
        StartMove(characterRightUI);
        StopMove(characterLeftUI);

        if (characterRightUI != null) characterRightUI.color = new Color(1, 1, 1, 1f);
        if (characterLeftUI != null) characterLeftUI.color = new Color(1, 1, 1, 1f);
    }

    void UpdateUI()
    {
        StoryPage currentPage = allPages[currentIndex];

        if (dialogueTextUI != null) 
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence(currentPage.dialogueText));
        }

        if (speakerNameUI != null) speakerNameUI.text = currentPage.speakerName;
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

        // +++ เพิ่มการเรียกใช้แอนิเมชันขยับตัวละคร (จากโค้ดใหม่) +++
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

                gameResult.ShowVictoryResultDirect(stars);
                SendResult(stars);
            }

            if (nextButton != null) nextButton.SetActive(false);
        }
    }
    IEnumerator PlayVSEffectAndStartQuiz()
    {
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

        if (page.characterCenter != null && characterCenterUI != null) SetupCharacter(characterCenterUI, page.characterCenter);
        else
        {
            if (page.characterLeft != null && characterLeftUI != null) SetupCharacter(characterLeftUI, page.characterLeft);
            if (page.characterRight != null && characterRightUI != null) SetupCharacter(characterRightUI, page.characterRight);
        }
    }

    void SetupCharacter(Image characterUI, Sprite characterSprite)
    {
        characterUI.sprite = characterSprite;
        characterUI.gameObject.SetActive(true);
    
    }

   void StartMove(Image character)
   {
       if (character == null) return;

        CharacterBounce bounce = character.GetComponent<CharacterBounce>();
        if (bounce == null)
        {
            bounce = character.gameObject.AddComponent<CharacterBounce>();
            bounce = character.gameObject.AddComponent<CharacterBounce>();
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
}