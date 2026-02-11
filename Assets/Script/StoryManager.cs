using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class StoryManager : MonoBehaviour
{
    public Text dialogueTextUI;
    public Text speakerNameUI;
    public Image backgroundImageUI;
    public Image characterImageUI;
    public GameObject nextButton; 
    public List<StoryPage> allPages; 
    private int currentIndex = 0;

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

        dialogueTextUI.text = currentPage.dialogueText;
        speakerNameUI.text = currentPage.speakerName;

        if (currentPage.background != null)
        {
            backgroundImageUI.sprite = currentPage.background;
        }

        if (currentPage.character != null)
        {
            characterImageUI.sprite = currentPage.character;
            characterImageUI.gameObject.SetActive(true);
        }
        else
        {
            characterImageUI.gameObject.SetActive(false); 
        }
    }
}