using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public enum PopupSide 
{ 
    Player, 
    Enemy 
}

public class Examlogic : MonoBehaviour // ใช้ชื่อคลาสเดิมเพื่อไม่ให้ Unity Inspector หลุด
{
    [System.Serializable]
    public class QuestionStore
    {
        [TextArea(3, 5)] public string questionText; 
        public QuestionType type; 
        public List<string> choices;
        public int correctChoiceIndex; 
        public string correctStringAnswer; 

        [Header("Feedback Messages (ข้อความตอนตอบถูก/ผิด)")]
        [TextArea(2, 3)] public string correctMessage; // ข้อความถ้าตอบถูก
        public PopupSide correctMessageSide;           // เลือกฝั่ง (Player/Enemy)

        [TextArea(2, 3)] public string wrongMessage;   // ข้อความถ้าตอบผิด
        public PopupSide wrongMessageSide;
    }

    [Header("UI Panels")]
    public GameObject questionPanel;
    public GameObject choicePanel; 
    public GameObject inputPanel;  

    [Header("UI Elements")]
    public Text questionTextUI;
    public Button[] choiceButtons; 
    public Text[] choicebuttontextUI; 
    public TMP_InputField answerInputField; 
    public Text resultTextUI;

    [Header("Exam Data")]
    public List<QuestionStore> allquestions;
    private int questionindex = 0;
    private QuestionStore currentquestion;
    private bool isProcessing = false;

    [Header("Feedback UI (กล่องข้อความตอนตอบ)")]
    public GameObject playerFeedbackPanel; // กล่องข้อความฝั่งผู้เล่น
    public Text playerFeedbackText;        // Text ในกล่องฝั่งผู้เล่น
    public GameObject enemyFeedbackPanel;  // กล่องข้อความฝั่งศัตรู
    public Text enemyFeedbackText;

    [Header("Heart Settings")]
    public int maxheart = 3;
    public int playercurrentheart;
    public int enemycurrentheart;
    public GameObject[] playerhearts;
    public GameObject[] enemyhearts;

    [Header("Screen Shake Settings")]
    [SerializeField] private RectTransform rectToShake; 
    [SerializeField] private float shakeDuration = 0.3f; 
    [SerializeField] private float shakeMagnitude = 10f;  
    private Vector2 originalPosition;  
    private Coroutine shakeCoroutine;

    [Header("Sound Reference")]
    public AudioSource soundEffect; 
    public AudioClip correctSound; 
    public AudioClip wrongSound;

    [Header("Game Systems")]
    public StoryManager storyManager;
    public GameResult gameResult;

    [Header("Achievements")]
    public int achievementIdToUnlock;
    public string achievementCodeToUnlock;

    void Start()
    {
        // เก็บตำแหน่งเริ่มต้นของจอเพื่อใช้ตอนสั่น
        if (rectToShake == null && questionPanel != null)
        {
            rectToShake = questionPanel.GetComponent<RectTransform>();
        }

        if (rectToShake != null)
        {
            originalPosition = rectToShake.anchoredPosition;
        }
        HideFeedback();
    }

    public void StartExam() 
    {
        this.gameObject.SetActive(true);
        if(questionPanel != null) questionPanel.SetActive(true);

        // เปิด UI เลือด
        if (playerhearts.Length > 0 && playerhearts[0].transform.parent != null)
            playerhearts[0].transform.parent.gameObject.SetActive(true);
    
        if (enemyhearts.Length > 0 && enemyhearts[0].transform.parent != null)
            enemyhearts[0].transform.parent.gameObject.SetActive(true);
            
        playercurrentheart = maxheart;
        enemycurrentheart = maxheart;
        isProcessing = false;

        questionindex = 0; 
        ShuffleQuestions(); // สุ่มคำถามใหม่ทุกครั้งที่เริ่ม

        if (rectToShake != null) rectToShake.anchoredPosition = originalPosition;
        HideFeedback();

        updateHeartUI();
        loadQuestion();
    }

    void loadQuestion()
    {
        if(questionindex >= allquestions.Count) return;

        currentquestion = allquestions[questionindex];
        if(questionTextUI != null) questionTextUI.text = currentquestion.questionText;

        if (currentquestion.type == QuestionType.MultipleChoice)
        {
            if(choicePanel != null) choicePanel.SetActive(true);
            if(inputPanel != null) inputPanel.SetActive(false);

            for(int i = 0; i < choicebuttontextUI.Length; i++)
            {
                // แก้บั๊กตรงนี้จาก .Length เป็น .Count
                if(i < currentquestion.choices.Count)
                    choicebuttontextUI[i].text = currentquestion.choices[i];
            }
        }
        else if (currentquestion.type == QuestionType.TextInput)
        {
            if(choicePanel != null) choicePanel.SetActive(false);
            if(inputPanel != null) inputPanel.SetActive(true);
            
            if(answerInputField != null) answerInputField.text = ""; 
        }

        if (storyManager != null) storyManager.SetOnlyEnemySpeaking();
    }

    // ฟังก์ชันรับคำตอบจากปุ่มช้อยส์
    public void Onanswerselected(int index)
    {
        if (currentquestion.type != QuestionType.MultipleChoice || isProcessing) return;
        ProcessResult(index == currentquestion.correctChoiceIndex);
    }

    // ฟังก์ชันรับคำตอบจากการพิมพ์
    public void OnSubmitInput()
    {
        if (currentquestion.type != QuestionType.TextInput || isProcessing) return;
        string playerAnswer = answerInputField.text.Trim();
        ProcessResult(playerAnswer == currentquestion.correctStringAnswer);
    }

    // ฟังก์ชันประมวลผลคำตอบ (รวมเรื่องเสียงและสั่นจอไว้ที่นี่)
    void ProcessResult(bool isCorrect)
    {
        IEnumerator DashAnimation(RectTransform characterTarget, float distanceX)
        {
            Vector2 startPos = characterTarget.anchoredPosition;
            Vector2 targetPos = startPos + new Vector2(distanceX, 0); // ระยะที่จะพุ่งไป
            float duration = 0.1f; // ความเร็วตอนพุ่ง

            // จังหวะพุ่งไปข้างหน้า
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                characterTarget.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / duration);
                yield return null;
            }

            // จังหวะเด้งกลับที่เดิม
            time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                characterTarget.anchoredPosition = Vector2.Lerp(targetPos, startPos, time / duration);
                yield return null;
            }

            // จัดให้ตรงจุดเป๊ะๆ ตอนจบ
            characterTarget.anchoredPosition = startPos;
        }

        // แอนิเมชันกระพริบสีแดงตอนโดนตี
        IEnumerator HurtAnimation(Image characterImage)
        {
            Color originalColor = Color.white;
            characterImage.color = new Color(1f, 0.3f, 0.3f); // เปลี่ยนเป็นสีแดงอ่อนๆ

            yield return new WaitForSeconds(0.15f); // ค้างสีแดงไว้แป๊บนึง

            characterImage.color = originalColor; // กลับเป็นสีเดิม
        }
        if(playercurrentheart <= 0 || enemycurrentheart <= 0) return;
        isProcessing = true;

        if(isCorrect)
        {
            Debug.Log("ตอบถูก!");
            if (soundEffect != null && correctSound != null) soundEffect.PlayOneShot(correctSound);
            enemycurrentheart--;
            if (storyManager != null)
            {
                if (storyManager.characterLeftUI != null) 
                    StartCoroutine(DashAnimation(storyManager.characterLeftUI.rectTransform, 50f));
                
                if (storyManager.characterRightUI != null) 
                    StartCoroutine(HurtAnimation(storyManager.characterRightUI));
            }
            TriggerShake(); 
            ShowFeedback(currentquestion.correctMessage, currentquestion.correctMessageSide);
        } 
        else 
        {
            Debug.Log("ตอบผิด!");
            if (soundEffect != null && wrongSound != null) soundEffect.PlayOneShot(wrongSound);
            playercurrentheart--;
            if (storyManager != null)
            {
                if (storyManager.characterRightUI != null) 
                    StartCoroutine(DashAnimation(storyManager.characterRightUI.rectTransform, -50f));
                
                if (storyManager.characterLeftUI != null) 
                    StartCoroutine(HurtAnimation(storyManager.characterLeftUI));
            }
            TriggerShake(); 
            ShowFeedback(currentquestion.wrongMessage, currentquestion.wrongMessageSide);
        }

        updateHeartUI();
        Invoke("HandleNextStep", 2f);
        
    }

    void ShowFeedback(string message, PopupSide side)
    {
        HideFeedback(); // ปิดของเก่าก่อน

        if (string.IsNullOrEmpty(message)) return; // ถ้าไม่ได้ใส่ข้อความไว้ ก็ไม่ต้องเปิดกล่อง

        if (side == PopupSide.Player)
        {
            if (playerFeedbackPanel != null) playerFeedbackPanel.SetActive(true);
            if (playerFeedbackText != null) playerFeedbackText.text = message;
        }
        else if (side == PopupSide.Enemy)
        {
            if (enemyFeedbackPanel != null) enemyFeedbackPanel.SetActive(true);
            if (enemyFeedbackText != null) enemyFeedbackText.text = message;
        }
    }

    // +++ ฟังก์ชันสำหรับซ่อนกล่องข้อความ +++
    void HideFeedback()
    {
        if (playerFeedbackPanel != null) playerFeedbackPanel.SetActive(false);
        if (enemyFeedbackPanel != null) enemyFeedbackPanel.SetActive(false);
    }

    void HandleNextStep()
    {
        HideFeedback();
        isProcessing = false; 
        CheckGameEnd();

        if (playercurrentheart > 0 && enemycurrentheart > 0 && questionindex < allquestions.Count - 1)
        {
            questionindex++;
            loadQuestion();
        }
    }

    void updateHeartUI()
    {
        for(int i = 0 ; i < playerhearts.Length; i++)
        {
            if(playerhearts[i] != null)
                playerhearts[i].SetActive(i < playercurrentheart);
        }

        for(int i = 0 ; i < enemyhearts.Length; i++)
        {
            if(enemyhearts[i] != null)
                enemyhearts[i].SetActive(i < enemycurrentheart);
        }
    }

    void CheckGameEnd()
    {
        if(playercurrentheart <= 0) 
        {
            Debug.Log("คุณแพ้");
            isProcessing = false;
            CloseExamUI();
            if (gameResult != null) gameResult.TriggerDefeat();
        }
        else if(enemycurrentheart <= 0) 
        {
            Debug.Log("คุณชนะ");
            CloseExamUI();

            if(achievementIdToUnlock > 0 && !string.IsNullOrEmpty(achievementCodeToUnlock)){
                AchievementManager.Instance.UnlockAchievement((uint)achievementIdToUnlock, achievementCodeToUnlock);
            }

            if (gameResult != null) gameResult.TriggerVictory(storyManager);
        }
    }

    void CloseExamUI()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        if (inputPanel != null) inputPanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        
        if (playerhearts.Length > 0 && playerhearts[0].transform.parent != null)
            playerhearts[0].transform.parent.gameObject.SetActive(false);
            
        if (enemyhearts.Length > 0 && enemyhearts[0].transform.parent != null)
            enemyhearts[0].transform.parent.gameObject.SetActive(false);
        
        HideFeedback();
        this.gameObject.SetActive(false);
    }
    
    void ShuffleQuestions()
    {
        for (int i = 0; i < allquestions.Count; i++)
        {
            int randomIndex = Random.Range(i, allquestions.Count);
            QuestionStore temp = allquestions[i];
            allquestions[i] = allquestions[randomIndex];
            allquestions[randomIndex] = temp;
        }
    }

    public void TriggerShake()
    {
        if (rectToShake == null) return;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            rectToShake.anchoredPosition = originalPosition + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null; 
        }
        rectToShake.anchoredPosition = originalPosition;
        shakeCoroutine = null;
    }
}