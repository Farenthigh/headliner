using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Examlogic : MonoBehaviour
{
    public int maxheart = 3;
    public int playercurrentheart;
    public int enemycurrentheart;
    private int questionindex = 0;
    public List<QuestionStore> allquestions;
    private QuestionStore currentquestion;
    public Text questiontextUI;
    public GameObject[] playerhearts;
    public GameObject[] enemyhearts;
    public GameObject choicePanel; 
    public Text[] choicebuttontextUI;
    public GameObject inputPanel; 
    public InputField answerInputField; 
    
    void Start() 
    {
        playercurrentheart = maxheart;
        enemycurrentheart = maxheart;
        questionindex = 0; 

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
        CheckGameEnd();

        if(playercurrentheart > 0 && enemycurrentheart > 0)
        {
            questionindex++;
            loadQuestion();
        }
    }

    void updateHeartUI()
    {
        for(int i = 0 ; i < playerhearts.Length; i++)
            playerhearts[i].SetActive(i < playercurrentheart);

        for(int i = 0 ; i < enemyhearts.Length; i++)
            enemyhearts[i].SetActive(i < enemycurrentheart);
    }

    void CheckGameEnd()
    {
        if(playercurrentheart <= 0) Debug.Log("คุณแพ้");
        else if(enemycurrentheart <= 0) Debug.Log("คุณชนะ");
    }
}