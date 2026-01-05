using UnityEngine;
using UnityEngine.UI;

public class InvestGameMap : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;

    private void Start()
    {
        button1.onClick.AddListener(() => SelectStage(1));
        button2.onClick.AddListener(() => SelectStage(2));
        button3.onClick.AddListener(() => SelectStage(3));
        button4.onClick.AddListener(() => SelectStage(4));
    }

    private void SelectStage(int stageNumber)
    {
        Debug.Log("Selected Stage: " + stageNumber);
    }
}
