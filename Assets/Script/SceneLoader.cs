using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSavingTips()
    {
        if (GameProgress.hardUnlocked)
            SceneManager.LoadScene("SavingGameHardTips");
        else
            SceneManager.LoadScene("SavingGameEasyTips");
    }
}
