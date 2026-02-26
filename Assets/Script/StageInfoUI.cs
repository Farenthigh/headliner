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
    if (data == null)
    {
        Debug.LogError("StageData is null!");
        return;
    }

    currentLevel = level;

    if (stageTitle != null)
        stageTitle.text = string.IsNullOrEmpty(data.stageName) ? "No Title" : data.stageName;

    if (mission1 != null)
        mission1.text = string.IsNullOrEmpty(data.mission1) ? "-" : data.mission1;

    if (mission2 != null)
        mission2.text = string.IsNullOrEmpty(data.mission2) ? "-" : data.mission2;

    if (mission3 != null)
        mission3.text = string.IsNullOrEmpty(data.mission3) ? "-" : data.mission3;
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