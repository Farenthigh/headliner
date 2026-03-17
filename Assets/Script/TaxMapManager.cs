using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaxMapManager : MonoBehaviour
{
    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;
    [SerializeField] private Button stage4Button;
    [SerializeField] private Button stage5Button;
    [SerializeField] private Button backButton;

    private void Start()
    {
        stage1Button.onClick.AddListener(OpenStage1);
        stage2Button.onClick.AddListener(OpenStage2);
        stage3Button.onClick.AddListener(OpenStage3);
        stage4Button.onClick.AddListener(OpenStage4);
        stage5Button.onClick.AddListener(OpenStage5);
        backButton.onClick.AddListener(() => SceneManager.LoadScene("SelectGame"));
    }

    public void OpenStage1()
    {
        StageData.selectedStage = 1;
        SceneManager.LoadScene("TaxStageInfo");
    }

    public void OpenStage2()
    {
        StageData.selectedStage = 2;
        SceneManager.LoadScene("TaxStageInfo");
    }

    public void OpenStage3()
    {
        StageData.selectedStage = 3;
        SceneManager.LoadScene("TaxStageInfo");
    }

    public void OpenStage4()
    {
        StageData.selectedStage = 4;
        SceneManager.LoadScene("TaxStageInfo");
    }

    public void OpenStage5()
    {
        StageData.selectedStage = 5;
        SceneManager.LoadScene("TaxStageInfo");
    }
}