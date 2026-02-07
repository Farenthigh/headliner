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
    public Text[] choicebuttontextUI;
    public GameObject[] playerhearts;
    public GameObject[] enemyhearts;

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
            Debug.Log("question out!!!!!!!!");
            return;
        }

        currentquestion = allquestions[questionindex];

        questiontextUI.text = currentquestion.questionText;
        
        for(int i = 0; i < choicebuttontextUI.Length; i++)
        {
            if(i < currentquestion.choices.Length)
            {
                choicebuttontextUI[i].text = currentquestion.choices[i];
            } 
        }
    }

    public void Onanswerselected(int index)
    {
        if(playercurrentheart <= 0 || enemycurrentheart <= 0)
        {
            Debug.Log("Game over");
            return;
        }

        if(index == currentquestion.correctchoiceindex)
        {
            Debug.Log("Corect");
            enemycurrentheart--;
        } 
        else 
        {
            Debug.Log("Incorrect");
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
        {
            playerhearts[i].SetActive(i < playercurrentheart);
        }

        for(int i = 0 ; i < enemyhearts.Length; i++)
        {
            enemyhearts[i].SetActive(i < enemycurrentheart);
        }
    }

    void CheckGameEnd()
    {
        if(playercurrentheart <= 0)
        {
            Debug.Log("u losee");
        } 
        else if(enemycurrentheart <= 0)
        {
            Debug.Log("u winn");
        }
    }
}