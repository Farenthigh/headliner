using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StageInfoManager : MonoBehaviour
{
    public TextMeshProUGUI stageTitle;
    public TextMeshProUGUI objective1;
    public TextMeshProUGUI objective2;
    public TextMeshProUGUI objective3;

    private string nextScene;

    void Start()
    {
        int stage = StageData.selectedStage;

        if (stage == 1)
        {
            ShowStageInfo(
                "Stage 1",
                "เรียนรู้ความหมายของภาษี",
                "เข้าใจ VAT 7%",
                "รู้รายได้ขั้นต่ำที่ต้องยื่นภาษี",
                "TaxGameStory1"
            );
        }
        else if (stage == 2)
        {
            ShowStageInfo(
                "Stage 2",
                "แยกประเภทเงินได้",
                "เข้าใจเงินเดือนแต่ละแบบ",
                "ระบุเงินได้ตามมาตรา 40",
                "TaxGameStory2"
            );
        }
        else if (stage == 3)
        {
            ShowStageInfo(
                "Stage 3",
                "ใช้ค่าลดหย่อนให้ถูก",
                "เข้าใจสิทธิประกันสังคม",
                "ใช้ค่าลดหย่อนเพื่อลดภาษี",
                "TaxGameStage3"
            );
        }
        else if (stage == 4)
        {
            ShowStageInfo(
                "Stage 4",
                "คำนวณเงินได้สุทธิ",
                "ใช้สูตรภาษีให้ถูก",
                "รู้ช่วงเงินได้ที่ไม่ต้องเสียภาษี",
                "TaxGameStage4"
            );
        }
        else if (stage == 5)
        {
            ShowStageInfo(
                "Stage 5",
                "เลือกแบบ ภ.ง.ด.",
                "ยื่นภาษีให้ทันเวลา",
                "จบภารกิจผู้เสียภาษี",
                "TaxGameStage5"
            );
        }
    }

    void ShowStageInfo(string title, string obj1, string obj2, string obj3, string sceneName)
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
        SceneManager.LoadScene("TaxGameMap");
    }

}