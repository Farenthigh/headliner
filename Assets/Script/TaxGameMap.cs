using UnityEngine;

public class TaxGameMap : MonoBehaviour
{

    [SerializeField] private Button stageButton1;
    [SerializeField] private Button stageButton2;
    [SerializeField] private Button stageButton3;
    [SerializeField] private Button stageButton4;
    [SerializeField] private Button stageButton5;

    private void Start()
    {
        stageButton1.onClick.AddListener(() => TaxGameMapOption("Stage 1"));
        stageButton2.onClick.AddListener(() => TaxGameMapOption("Stage 2"));
        stageButton3.onClick.AddListener(() => TaxGameMapOption("Stage 3"));
        stageButton4.onClick.AddListener(() => TaxGameMapOption("Stage 4"));
        stageButton5.onClick.AddListener(() => TaxGameMapOption("Stage 5"));
    }
    private void TaxGameMapOption(string stageNumber)
    {
        Debug.Log($"Tax Game Map Option Selected: {stageNumber}");
    }

}
