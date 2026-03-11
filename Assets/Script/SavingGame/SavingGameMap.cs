using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SavingGameMap : MonoBehaviour
{
    [Header("Top Buttons")]
    public Button backButton;
    public Button leaderboardButton;
    public Button achievementButton;
    public Button settingButton;

    [Header("Level Buttons")]
    public Button[] levelButtons; // ด่าน 1–5

    [Header("Level Images")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    private int unlockedLevel = 1;

    void Start()
    {
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // ===== ปุ่มด้านบน =====
        backButton.onClick.AddListener(OnBack);
        leaderboardButton.onClick.AddListener(OnLeaderboard);
        achievementButton.onClick.AddListener(OnAchievement);
        settingButton.onClick.AddListener(OnSetting);

        // ===== ปุ่มด่าน =====
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => OnLevelClick(levelIndex));
            UpdateLevelButton(levelButtons[i], levelIndex);
        }
    }

    // ================= ปุ่มด้านบน =================
    void OnBack()
    {
        Debug.Log("ย้อนกลับ");
        // อนาคต: SceneManager.LoadScene("MainMenu");
    }

    void OnLeaderboard()
    {
        Debug.Log("ไปหน้า Leaderboard");
        // อนาคต: SceneManager.LoadScene("Leaderboard");
    }

    void OnAchievement()
    {
        Debug.Log("ไปหน้า Achievement");
        // อนาคต: SceneManager.LoadScene("Achievement");
    }

    void OnSetting()
    {
        Debug.Log("ไปหน้า Setting");
        // อนาคต: SceneManager.LoadScene("Setting");
    }

    // ================= ปุ่มด่าน =================
    void OnLevelClick(int level)
    {
        Debug.Log("เลือกด่าน " + level);

        // ทดลองปลดล็อกด่านถัดไป
        if (level == unlockedLevel && unlockedLevel < levelButtons.Length)
        {
            unlockedLevel++;
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
            PlayerPrefs.Save();
        }

        RefreshAllLevels();

        // อนาคต:
        // SceneManager.LoadScene("Level" + level);
    }

    void RefreshAllLevels()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            UpdateLevelButton(levelButtons[i], i + 1);
        }
    }

    void UpdateLevelButton(Button button, int levelIndex)
    {
        bool isUnlocked = levelIndex <= unlockedLevel;
        button.interactable = isUnlocked;

        Image img = button.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = isUnlocked ? unlockedSprite : lockedSprite;
        }
    }
}
