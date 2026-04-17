using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.PlayMusic("theme");
    }
    public void PlayGame()
    {
        if (APIManager.myData.character == 0 && APIManager.myData.username == "")
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCharacterScene");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectGame");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("LoginScene");
    }
}