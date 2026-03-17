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
   public void SetOnlyEnemySpeaking()
   {
       StartMove(characterRightUI);
       StopMove(characterLeftUI);


       characterRightUI.color = new Color(1,1,1,1f);
       characterLeftUI.color = new Color(1,1,1,1f);
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
       if (!currentPage.isChoicePage)
       {
           UpdateCharacterAnimation(currentPage);
       }


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




       if (currentIndex == allPages.Count - 1)
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


   void StartMove(Image character)
   {
       if (character == null) return;


       CharacterBounce bounce = character.GetComponent<CharacterBounce>();


       if (bounce == null)
       {
           bounce = character.gameObject.AddComponent<CharacterBounce>();
       }


       bounce.enabled = true; // ✅ เปิดใช้งาน
   }  
   void StopMove(Image character)
   {
       if (character == null) return;


       CharacterBounce bounce = character.GetComponent<CharacterBounce>();
       if (bounce != null)
       {
           bounce.enabled = false; // ❗ เปลี่ยนจาก Destroy → Disable
       }
   }
   void UpdateCharacterAnimation(StoryPage page)
   {


       Debug.Log("Animating: " + page.speakerPosition);
   // 💥 ลบของเก่าทิ้งก่อนทุกครั้ง
       StopMove(characterLeftUI);
       StopMove(characterCenterUI);
       StopMove(characterRightUI);


   // 💥 force reset position (สำคัญมาก)
       ResetPosition(characterLeftUI);
       ResetPosition(characterCenterUI);
       ResetPosition(characterRightUI);


   // 🎯 เริ่มใหม่
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
  
}

