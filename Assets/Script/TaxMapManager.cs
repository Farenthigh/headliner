using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class TaxMapManager : MonoBehaviour
{
    public Button stage1;
    public Button stage2;
    public Button stage3;
    public Button stage4;
    public Button stage5;

    public GameObject stageInfoimage;
    public GameObject stageInfoPanel;
    public StageInfoManager stageInfoManager;

    public Transform stage1Stars;
    public Transform stage2Stars;
    public Transform stage3Stars;
    public Transform stage4Stars;
    public Transform stage5Stars;

    private const string LAST_STAGE_KEY = "LastPlayedStage";

    async void Start()
    {   
        stageInfoimage.SetActive(false);
        stageInfoPanel.SetActive(false);

        // รีเซ็ตดาว
        SetStars(stage1Stars, 0);
        SetStars(stage2Stars, 0);
        SetStars(stage3Stars, 0);
        SetStars(stage4Stars, 0);
        SetStars(stage5Stars, 0);

        // โหลดดาว
        await stageInfoManager.LoadStars();
        ShowStars();

        var unlock = await APIManager.Instance.GetStageUnlock();

        int nextStage = 1;
        int nowStage = 0;

        if (unlock.next_stage > 0)
            nextStage = unlock.next_stage;

        if (unlock.now_stage > 0)
            nowStage = unlock.now_stage;

        // ปลดล็อกปุ่ม
        stage1.interactable = nextStage >= 1;
        stage2.interactable = nextStage >= 2;
        stage3.interactable = nextStage >= 3;
        stage4.interactable = nextStage >= 4;
        stage5.interactable = nextStage >= 5;

        // ทำดาวจาง
        if (nextStage < 1) SetStarAlpha(stage1Stars, 0.3f);
        if (nextStage < 2) SetStarAlpha(stage2Stars, 0.3f);
        if (nextStage < 3) SetStarAlpha(stage3Stars, 0.3f);
        if (nextStage < 4) SetStarAlpha(stage4Stars, 0.3f);
        if (nextStage < 5) SetStarAlpha(stage5Stars, 0.3f);

        // =========================
        // เด้งเฉพาะด่านที่ "เพิ่งผ่านใหม่"
        // =========================
       
        int lastStage = PlayerPrefs.GetInt(LAST_STAGE_KEY,1);

        if (!PlayerPrefs.HasKey(LAST_STAGE_KEY))
        {
            // เข้าเกมครั้งแรก → เด้ง stage1
            StartCoroutine(PlayStageFlow(1, nowStage));
        }

        else if (nowStage > lastStage)
        {
            // ผ่านด่านใหม่ → เด้งเฉพาะด่านใหม่
            StartCoroutine(PlayStageFlow(lastStage + 1, nowStage));
        }
        
        Debug.Log("lastStage = " + lastStage);
        Debug.Log("nowStage = " + nowStage);
        PlayerPrefs.SetInt(LAST_STAGE_KEY, nowStage);
        PlayerPrefs.Save();
    }

    // =========================
    // 🎯 Flow Animation
    // =========================
    IEnumerator PlayStageFlow(int start, int end)
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = start; i <= end; i++)
        {
            PlayUnlockBounce(i);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PlayUnlockBounce(int stage)
    {
        Transform target = null;
        Transform starGroup = null;

        if (stage == 1) { target = stage1.transform; starGroup = stage1Stars; }
        if (stage == 2) { target = stage2.transform; starGroup = stage2Stars; }
        if (stage == 3) { target = stage3.transform; starGroup = stage3Stars; }
        if (stage == 4) { target = stage4.transform; starGroup = stage4Stars; }
        if (stage == 5) { target = stage5.transform; starGroup = stage5Stars; }

        if (target != null)
            StartCoroutine(BounceEffect(target));

        if (starGroup != null)
            StartCoroutine(BounceStars(starGroup));
    }

    IEnumerator BounceStars(Transform starGroup)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform star = starGroup.Find("star" + i);

            if (star != null && star.gameObject.activeSelf)
            {
                StartCoroutine(BounceEffect(star));
                yield return new WaitForSeconds(0.08f);
            }
        }
    }

    void OpenStage(int stage)
    {   
        Transform btn = null;

        if (stage == 1) btn = stage1.transform;
        if (stage == 2) btn = stage2.transform;
        if (stage == 3) btn = stage3.transform;
        if (stage == 4) btn = stage4.transform;
        if (stage == 5) btn = stage5.transform;

        if (btn != null)
            StartCoroutine(BounceEffect(btn));

        stageInfoimage.SetActive(true);
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
        if (stageInfoManager.playerStars == null) return;

        foreach (var s in stageInfoManager.playerStars)
        {
            Transform target = null;

            if (s.Stage == 1) target = stage1Stars;
            if (s.Stage == 2) target = stage2Stars;
            if (s.Stage == 3) target = stage3Stars;
            if (s.Stage == 4) target = stage4Stars;
            if (s.Stage == 5) target = stage5Stars;

            if (target != null)
                SetStars(target, s.Stars);
        }
    }

    void SetStars(Transform stage, int starCount)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform star = stage.Find("star" + i);

            if (star != null)
            {
                star.gameObject.SetActive(i <= starCount);
            }
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

    // =========================
    // Bounce Animation
    // =========================
    IEnumerator BounceEffect(Transform target)
    {
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
    }

    public void GoToSelectGame()
    {
        LoadingManager.Instance.LoadScene("SelectGame");
    }
}