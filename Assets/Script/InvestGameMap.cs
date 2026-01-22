using UnityEngine;
using UnityEngine.UI;

public class InvestGameMap : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [SerializeField] private Button button5;
    [SerializeField] private Button achievement;
    [SerializeField] private Button leaderboard;
    [SerializeField] private Button Setting;

    private void Start()
    {
        button1.onClick.AddListener(() => SelectStage(1));
        button2.onClick.AddListener(() => SelectStage(2));
        button3.onClick.AddListener(() => SelectStage(3));
        button4.onClick.AddListener(() => SelectStage(4));
        button5.onClick.AddListener(() => SelectStage(5));
        achievement.onClick.AddListener(() => SelectItem("Achievement"));
        leaderboard.onClick.AddListener(() => SelectItem("Leaderboard"));
        Setting.onClick.AddListener(() => SelectItem("Setting"));
    }

    private void SelectStage(int stageNumber)
    {
        Debug.Log("Selected Stage: " + stageNumber);
    }
    private void SelectItem(string stage)
    {
        Debug.Log("Go to" + stage);
    }
    private void SelectSetting (string stage)
    {
        Debug.Log("Go to" + stage);
    }
    
}
