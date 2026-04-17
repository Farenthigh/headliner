using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string settingSceneName = "SettingsScene"; 

    public void GoToSettings()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetString("PreviousScene", currentSceneName);
        PlayerPrefs.Save();

        LoadingManager.Instance.LoadScene(settingSceneName);
    }
}