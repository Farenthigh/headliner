using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageInfoUI : MonoBehaviour
{
    public TMP_Text stageTitle;
    public TMP_Text mission1;
    public TMP_Text mission2;
    public TMP_Text mission3;

  

    private int currentLevel;

    public void Setup(StageData data, int level)
    {
        currentLevel = level;

        stageTitle.text = data.stageName;
        mission1.text = data.mission1;
        mission2.text = data.mission2;
        mission3.text = data.mission3;

    }

    public void StartLevel()
    {
        Debug.Log("Loading Level: " + currentLevel);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}