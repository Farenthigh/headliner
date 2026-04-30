using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (APIManager.myData.character == 0 && APIManager.myData.username == "")
            LoadingManager.Instance.LoadScene("SelectCharacterScene");
        else
            LoadingManager.Instance.LoadScene("SelectGame");
    }

    public void OpenSettings()
    {
        LoadingManager.Instance.LoadScene("SettingsScene");
    }

    public void ExitGame()
    {
        LoadingManager.Instance.LoadScene("LoginScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}