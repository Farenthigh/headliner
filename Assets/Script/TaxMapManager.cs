using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TaxMapManager : MonoBehaviour
{
    public Button stage1;
    public Button stage2;
    public Button stage3;
    public Button stage4;
    public Button stage5;

    public GameObject stageInfoPanel;
    public StageInfoManager stageInfoManager;

    public Transform stage1Stars;
    public Transform stage2Stars;
    public Transform stage3Stars;
    public Transform stage4Stars;
    public Transform stage5Stars;

    async void Start()
    {
        stageInfoPanel.SetActive(false);

        SetStars(stage1Stars, 0);
        SetStars(stage2Stars, 0);
        SetStars(stage3Stars, 0);
        SetStars(stage4Stars, 0);
        SetStars(stage5Stars, 0);

        // โหลดดาวก่อน
        await stageInfoManager.LoadStars();

        ShowStars();

        var unlock = await APIManager.Instance.GetStageUnlock();
        int nextStage = unlock.next_stage;

        stage1.interactable = nextStage >= 1;
        stage2.interactable = nextStage >= 2;
        stage3.interactable = nextStage >= 3;
        stage4.interactable = nextStage >= 4;
        stage5.interactable = nextStage >= 5;

        // ทำดาวจางถ้ายังไม่ปลดล็อก
        if (nextStage < 1) SetStarAlpha(stage1Stars, 0.3f);
        if (nextStage < 2) SetStarAlpha(stage2Stars, 0.3f);
        if (nextStage < 3) SetStarAlpha(stage3Stars, 0.3f);
        if (nextStage < 4) SetStarAlpha(stage4Stars, 0.3f);
        if (nextStage < 5) SetStarAlpha(stage5Stars, 0.3f);
    }

    void OpenStage(int stage)
    {
        stageInfoPanel.SetActive(true);
        stageInfoManager.ShowStage(stage);
    }

    public void OpenStage1() => OpenStage(1);
    public void OpenStage2() => OpenStage(2);
    public void OpenStage3() => OpenStage(3);
    public void OpenStage4() => OpenStage(4);
    public void OpenStage5() => OpenStage(5);

    void ShowStars()
    {   
        // ปิดดาวทุกด่านก่อน (กัน null จาก server)
        SetStars(stage1Stars, 0);
        SetStars(stage2Stars, 0);
        SetStars(stage3Stars, 0);
        SetStars(stage4Stars, 0);
        SetStars(stage5Stars, 0);

        if (stageInfoManager.playerStars == null) return;

        foreach (var s in stageInfoManager.playerStars)
        {
            if (s.Stage == 1) SetStars(stage1Stars, s.Stars);
            if (s.Stage == 2) SetStars(stage2Stars, s.Stars);
            if (s.Stage == 3) SetStars(stage3Stars, s.Stars);
            if (s.Stage == 4) SetStars(stage4Stars, s.Stars);
            if (s.Stage == 5) SetStars(stage5Stars, s.Stars);
        }
    }

    void SetStars(Transform stage, int starCount)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform star = stage.Find("star" + i);

            if (star != null)
                star.gameObject.SetActive(i <= starCount);
        }
    }

    void SetStarAlpha(Transform stage, float alpha)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform star = stage.Find("star" + i + "_d");

            if (star != null)
            {
                Image img = star.GetComponent<Image>();
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }

}