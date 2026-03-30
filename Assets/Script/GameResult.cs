using UnityEngine;
using UnityEngine.UI;   // ถ้าใช้ Text / Image
using System.Collections;
using UnityEngine.SceneManagement;
public class GameResult : MonoBehaviour
{
    [Header("Victory UI")]
    [SerializeField] private GameObject victoryIntroUI;
    [SerializeField] private GameObject victoryResultUI;
    [SerializeField] private GameObject Exit;
    [SerializeField] private GameObject Play_again;
    [SerializeField] private GameObject Next_game;

    [Header("Defeat UI")]
    [SerializeField] private GameObject defeatIntroUI;
    [SerializeField] private GameObject defeatResultUI;
    [SerializeField] private GameObject d_Exit;
    [SerializeField] private GameObject d_Play_again;


    [Header("Victory Stars")]
    [SerializeField] private GameObject v_star1;
    [SerializeField] private GameObject v_star2;
    [SerializeField] private GameObject v_star3;

    [SerializeField] private GameObject v_priority1;
    [SerializeField] private GameObject v_priority2;
    [SerializeField] private GameObject v_priority3;

    [Header("Defeat Stars")]
    [SerializeField] private GameObject d_star1;
    [SerializeField] private GameObject d_star2;
    [SerializeField] private GameObject d_star3;

    [Header("Next Level")]
    [SerializeField] private string nextSceneName;



    private void Start()
    {
        victoryIntroUI.SetActive(false);
        victoryResultUI.SetActive(false);

        defeatIntroUI.SetActive(false);
        defeatResultUI.SetActive(false);

        if (!PlayerPrefs.HasKey("Tax_Level"))
        {
            PlayerPrefs.SetInt("Tax_Level", 1);
            PlayerPrefs.SetInt("Tax_Failed", 0);
            PlayerPrefs.Save();
        }
    }
    public void TriggerVictory(StoryManager storyManager)
    {
        gameObject.SetActive(true);
        StartCoroutine(VictoryFlow(storyManager));
    }

    public void TriggerDefeat()
    {
        gameObject.SetActive(true);   // 👈 เพิ่มบรรทัดนี้
        StartCoroutine(DefeatFlow());
        PlayerPrefs.SetInt("Tax_Failed", 1);
        PlayerPrefs.Save();
    }

    private IEnumerator VictoryFlow(StoryManager storyManager)
    {
        victoryIntroUI.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        victoryIntroUI.SetActive(false);

        // 👉 ค่อยเปลี่ยนหน้า Story ตอนนี้
        if (storyManager != null)
            storyManager.OnClickNext();
    }
    public void ShowVictoryResultDirect(int starCount)
    {
        // ✅ เพิ่ม level
        int level = PlayerPrefs.GetInt("Tax_Level", 1);
        level++;

        PlayerPrefs.SetInt("Tax_Level", level);
        PlayerPrefs.Save();

        // ✅ เช็ค achievement
        if (level > 5)
        {
        AchievementManager.Instance.CheckTaxComebackKing();
        }
        gameObject.SetActive(true);
        ShowVictoryStars(starCount);
        victoryResultUI.SetActive(true);
    }

    private IEnumerator DefeatFlow()
    {
        defeatIntroUI.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        defeatIntroUI.SetActive(false);
        ShowDefeatStars();
        defeatResultUI.SetActive(true);
    }

    private void ShowVictoryStars(int starCount)
    {
        v_star1.SetActive(starCount >= 1);
        v_star2.SetActive(starCount >= 2);
        v_star3.SetActive(starCount >= 3);

        v_priority1.SetActive(starCount >= 1);
        v_priority2.SetActive(starCount >= 2);
        v_priority3.SetActive(starCount >= 3);
    }
    // private void ShowVictoryStars()
    // {
    //     int starCount = 2;

    //     v_star1.SetActive(false);
    //     v_star2.SetActive(false);
    //     v_star3.SetActive(false);

    //     v_priority1.SetActive(false);
    //     v_priority2.SetActive(false);
    //     v_priority3.SetActive(false);

    //     if (starCount >= 1)
    //     {
    //         v_star1.SetActive(true);
    //         v_priority1.SetActive(true);
    //     }

    //     if (starCount >= 2)
    //     {
    //         v_star2.SetActive(true);
    //         v_priority2.SetActive(true);
    //     }

    //     if (starCount >= 3)
    //     {
    //         v_star3.SetActive(true);
    //         v_priority3.SetActive(true);
    //     }
    // }

    private void ShowDefeatStars()
    {
        d_star1.SetActive(true);
        d_star2.SetActive(true);
        d_star3.SetActive(true);
    }

    public void OnClickExit()
    {
    
        Debug.Log("Victory Exit Clicked");
        SceneManager.LoadScene("TaxGameMap");
    }

    public void OnClickPlayAgain()
    {   
        AchievementData ach = AchievementManager.Instance.allAchievements
            .Find(a => a.id == "24");

        if (ach != null && !ach.isUnlocked)
        {
            AchievementManager.Instance.UnlockAchievement(24, "24");
        }
        PlayerPrefs.SetInt("Tax_Level", 1);
        PlayerPrefs.SetInt("Tax_Failed", 0);
        PlayerPrefs.Save();
        Debug.Log("Victory Play Again Clicked");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnClickNextGame()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("Next Scene Name is EMPTY!");
            return;
        }

        // เช็คว่า Scene อยู่ใน Build Profiles หรือไม่
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Scene '" + nextSceneName + "' is NOT in Build Profiles!");
        }
    }


    // ===== Defeat Buttons =====
    public void OnClickDefeatExit()
    {
        Debug.Log("Defeat Exit Clicked");
        SceneManager.LoadScene("TaxGameMap");
    }

    public void OnClickDefeatPlayAgain()
    {
        Debug.Log("Defeat Play Again Clicked");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }



}