using System;
using UnityEngine;
using UnityEngine.UI;

public class SavingGameStage : MonoBehaviour
{
    [SerializeField] private int stage;
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Button stageButton;

    private int star = -1;

    public void Start()
    {
        GetStar();
    }

    private async void GetStar()
    {
        try
        {
            star = await APIManager.Instance.GetSavingGameStar(stage);
            int latestStage = await APIManager.Instance.GetlatestStage();
            Debug.Log($"Stage {stage} has {star} stars.");
            if (star == -1)
            {
                star1.gameObject.SetActive(false);
                star2.gameObject.SetActive(false);
                star3.gameObject.SetActive(false);
            }
            if (stage > latestStage)
            {
                stageButton.interactable = false;
            }
            if (star >= 1) star1.sprite = filledStar;
            if (star >= 2) star2.sprite = filledStar;
            if (star >= 3) star3.sprite = filledStar;
            Debug.Log($"Stage {stage} has {star} stars and display updated.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching star data: {ex.Message}");
        }
    }
}