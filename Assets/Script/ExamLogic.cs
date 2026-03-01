using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Examlogic : MonoBehaviour
{
    public int maxheart = 3;
    public int playercurrentheart;
    public int enemycurrentheart;
    private int questionindex = 0;
    private bool isProcessing = false;
    public List<QuestionStore> allquestions;
    private QuestionStore currentquestion;
    public Text questiontextUI;
    public GameObject[] playerhearts;
    public GameObject[] enemyhearts;
    public GameObject choicePanel; 
    public GameObject questionPanel;
    public Text[] choicebuttontextUI;
    public GameObject inputPanel; 
    public TMP_InputField answerInputField; 
    public StoryManager storyManager;
    public GameResult gameResult;

    public void StartExam() 
    {
        this.gameObject.SetActive(true);
        if(questionPanel != null) questionPanel.SetActive(true);

        //Set Auto Open SuperClass Heart
        if (playerhearts.Length > 0 && playerhearts[0].transform.parent != null)
        playerhearts[0].transform.parent.gameObject.SetActive(true);
    
        if (enemyhearts.Length > 0 && enemyhearts[0].transform.parent != null)
            enemyhearts[0].transform.parent.gameObject.SetActive(true);
            
        playercurrentheart = maxheart;
        enemycurrentheart = maxheart;

        updateHeartUI();
        loadQuestion();
    }

    void loadQuestion()
    {
        if(questionindex >= allquestions.Count)
        {
            Debug.Log("หมดแล้วคำถามหมด");
            return;
        }

        currentquestion = allquestions[questionindex];
        questiontextUI.text = currentquestion.questionText;

        if (currentquestion.type == QuestionType.MultipleChoice)
        {
            choicePanel.SetActive(true);
            inputPanel.SetActive(false);

            for(int i = 0; i < choicebuttontextUI.Length; i++)
            {
                if(i < currentquestion.choices.Length)
                    choicebuttontextUI[i].text = currentquestion.choices[i];
            }
        }
        else if (currentquestion.type == QuestionType.TextInput)
        {
            choicePanel.SetActive(false);
            inputPanel.SetActive(true);
            
            answerInputField.text = ""; 
        }
    }

    public void Onanswerselected(int index)
    {
        if (currentquestion.type != QuestionType.MultipleChoice) return;
        ProcessResult(index == currentquestion.correctChoiceIndex);
    }

    public void OnSubmitInput()
    {
        if (currentquestion.type != QuestionType.TextInput) return;

        string playerAnswer = answerInputField.text.Trim();
        
        bool isCorrect = playerAnswer == currentquestion.correctStringAnswer;
        
        ProcessResult(isCorrect);
    }

    void ProcessResult(bool isCorrect)
    {
        if(playercurrentheart <= 0 || enemycurrentheart <= 0) return;
        isProcessing = true;

        if(isCorrect)
        {
            Debug.Log("ตอบถูก!");
            enemycurrentheart--;
        } 
        else 
        {
            Debug.Log("ตอบผิด!");
            playercurrentheart--;
        }

        updateHeartUI();
        Invoke("HandleNextStep", 0.5f);
        // CheckGameEnd();

        // if(playercurrentheart > 0 && enemycurrentheart > 0)
        // {
        //     questionindex++;
        //     loadQuestion();
        // }
    }

    void HandleNextStep()
    {
        isProcessing = false; // ปลดล็อก
        CheckGameEnd();

        // ถ้ายังไม่จบเกม ให้โหลดข้อถัดไป
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
            {
                // ถ้าลำดับ i น้อยกว่าเลือดที่มี ให้เปิดดวงนั้น
                playerhearts[i].SetActive(i < playercurrentheart);
            }
        }

        // อัปเดตฝั่งศัตรู
        for(int i = 0 ; i < enemyhearts.Length; i++)
        {
            if(enemyhearts[i] != null)
            {
                enemyhearts[i].SetActive(i < enemycurrentheart);
            }
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
        else if(enemycurrentheart <= 0 || questionindex >= allquestions.Count - 1) 
        {
            Debug.Log("คุณชนะ");
            CloseExamUI();
            if (gameResult != null)
                gameResult.TriggerVictory(storyManager);
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

        this.gameObject.SetActive(false);
    }
}