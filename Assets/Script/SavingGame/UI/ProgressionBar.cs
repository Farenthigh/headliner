using TMPro;
using UnityEngine;

public class ProgressionBar : MonoBehaviour
{
    [SerializeField] private GameObject progressionBar;
    [SerializeField] private TextMeshProUGUI progressionText;
    private float barMaxWidth;
    private void Start()
    {
        barMaxWidth = progressionBar.GetComponent<RectTransform>().sizeDelta.x;
    }
    public void Update()
    {
        UpdateProgression(SavingGameLogicManager.Instance.GetCurrentCash(), SavingGameLogicManager.Instance.GetGoalAmount());
    }
    public void UpdateProgression(float current, float target)
    {
        float progress = Mathf.Clamp01(current / target);
        progressionBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barMaxWidth * progress, progressionBar.GetComponent<RectTransform>().sizeDelta.y);
        progressionText.text = $"{current}/{target}";
    }
}
