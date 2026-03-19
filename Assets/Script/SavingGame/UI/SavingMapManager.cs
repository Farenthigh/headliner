using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingMapManager : MonoBehaviour
{
    [SerializeField] private SavingStageInfoManager stageInfoManager;

    public void OpenStage1()
    {
        SavingStageData.selectedStage = 1;

        stageInfoManager.ShowStageInfo(
            "Stage 1",
            "มีทรัพย์สินทั้งหมด 12,300 บาท",
            "มีทรัพย์สินทั้งหมด 12,400 บาท",
            "มีทรัพย์สินทั้งหมด 12,500 บาท",
            "SavingGameStage1"
        );
        stageInfoManager.gameObject.SetActive(true);
    }

    public void OpenStage2()
    {
        SavingStageData.selectedStage = 2;
        stageInfoManager.ShowStageInfo(
            "Stage 2",
            "มีทรัพย์สินทั้งหมด 15,500 บาท",
            "มีทรัพย์สินทั้งหมด 15,600 บาท",
            "มีทรัพย์สินทั้งหมด 15,700 บาท",
            "SavingGameStage2"
        );
        stageInfoManager.gameObject.SetActive(true);
    }

    public void OpenStage3()
    {
        SavingStageData.selectedStage = 3;
        stageInfoManager.ShowStageInfo(
            "Stage 3",
            "มีทรัพย์สินทั้งหมด 22,500 บาท",
            "มีทรัพย์สินทั้งหมด 22,600 บาท",
            "มีทรัพย์สินทั้งหมด 22,700 บาท",
            "SavingGameStage3"
        );
        stageInfoManager.gameObject.SetActive(true);
    }

    public void OpenStage4()
    {
        SavingStageData.selectedStage = 4;
        stageInfoManager.ShowStageInfo(
            "Stage 4",
            "มีทรัพย์สินทั้งหมด 12,600 บาท",
            "มีทรัพย์สินทั้งหมด 12,700 บาท",
            "มีทรัพย์สินทั้งหมด 12,800 บาท",
            "SavingGameStage4"
        );
        stageInfoManager.gameObject.SetActive(true);
    }

    public void OpenStage5()
    {
        SavingStageData.selectedStage = 5;
        stageInfoManager.ShowStageInfo(
            "Stage 5",
            "มีทรัพย์สินทั้งหมด 18,500 บาท",
            "มีทรัพย์สินทั้งหมด 18,600 บาท",
            "มีทรัพย์สินทั้งหมด 18,700 บาท",
            "SavingGameStage5"
        );
        stageInfoManager.gameObject.SetActive(true);
    }
}