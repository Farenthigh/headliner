using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StageInfoUI : MonoBehaviour
{
    public TextMeshProUGUI stageTitle;
    public TextMeshProUGUI objective1;
    public TextMeshProUGUI objective2;
    public TextMeshProUGUI objective3;

    private string sceneName;

    public void ShowStageInfo(string title,string obj1,string obj2,string obj3,string scene)
    {
        stageTitle.text = title;
        objective1.text = obj1;
        objective2.text = obj2;
        objective3.text = obj3;

        sceneName = scene;

        gameObject.SetActive(true);
    }

    public void StartStage()
    {
        SceneManager.LoadScene(sceneName);
    }

    
}