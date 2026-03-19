using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavingStageInfoManager : MonoBehaviour
{
    public TextMeshProUGUI stageTitle;
    public TextMeshProUGUI objective1;
    public TextMeshProUGUI objective2;
    public TextMeshProUGUI objective3;
    public Button startButton;

    private string nextScene;

    void Start()
    {
        int stage = SavingStageData.selectedStage;
        gameObject.SetActive(false);
        if (stage == 1)
        {
            ShowStageInfo(
                "Stage 1",
                "มีทรัพย์สินทั้งหมด 12,300 บาท",
                "มีทรัพย์สินทั้งหมด 12,400 บาท",
                "มีทรัพย์สินทั้งหมด 12,500 บาท",
                "SavingGameStage1"
            );
        }
        else if (stage == 2)
        {
            ShowStageInfo(
                "Stage 2",
                "มีทรัพย์สินทั้งหมด 15,500 บาท",
                "มีทรัพย์สินทั้งหมด 15,600 บาท",
                "มีทรัพย์สินทั้งหมด 15,700 บาท",
                "SavingGameStage2"
            );
        }
        else if (stage == 3)
        {
            ShowStageInfo(
                "Stage 3",
                "มีทรัพย์สินทั้งหมด 22,500 บาท",
                "มีทรัพย์สินทั้งหมด 22,600 บาท",
                "มีทรัพย์สินทั้งหมด 22,700 บาท",
                "SavingGameStage3"
            );
        }
        else if (stage == 4)
        {
            ShowStageInfo(
                "Stage 4",
                "มีทรัพย์สินทั้งหมด 12,600 บาท",
                "มีทรัพย์สินทั้งหมด 12,700 บาท",
                "มีทรัพย์สินทั้งหมด 12,800 บาท",
                "SavingGameStage4"
            );
        }
        else if (stage == 5)
        {
            ShowStageInfo(
                "Stage 5",
                "มีทรัพย์สินทั้งหมด 18,500 บาท",
                "มีทรัพย์สินทั้งหมด 18,600 บาท",
                "มีทรัพย์สินทั้งหมด 18,700 บาท",
                "SavingGameStage5"
            );
        }
        startButton.onClick.AddListener(StartStage);
    }

    public void ShowStageInfo(string title, string obj1, string obj2, string obj3, string sceneName)
    {
        stageTitle.text = title;
        objective1.text = obj1;
        objective2.text = obj2;
        objective3.text = obj3;

        nextScene = sceneName;
    }

    public void StartStage()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void CloseStageInfo()
    {
        SceneManager.LoadScene("SavingGameMap");
    }

}