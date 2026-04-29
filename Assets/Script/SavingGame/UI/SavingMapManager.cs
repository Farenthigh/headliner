using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavingMapManager : MonoBehaviour
{
    [SerializeField] private SavingStageInfoManager stageInfoManager;

    public void OpenStage1()
    {
        SavingStageData.selectedStage = 1;
        StartCoroutine(BounceEffect(stageInfoManager.transform));
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

    IEnumerator BounceEffect(Transform target)
    {
        gameObject.GetComponent<Button>().interactable = false; // ปิดการกดปุ่มระหว่างแอนิเมชัน
        Vector3 original = target.localScale;
        Vector3 big = original * 1.25f; 

        float duration = 0.15f;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.Sin((time / duration) * Mathf.PI * 0.6f);
            target.localScale = Vector3.Lerp(original, big, t);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        while (time < duration)
        {
            float t = 1 - Mathf.Cos((time / duration) * Mathf.PI * 0.6f);
            target.localScale = Vector3.Lerp(big, original, t);

            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = original;
        gameObject.GetComponent<Button>().interactable = true; // เปิดการกดปุ่มหลังแอนิเมชัน
    }

    public void BackToSelectGame()
    {
        LoadingManager.Instance.LoadScene("SelectGame");
    }
    
}