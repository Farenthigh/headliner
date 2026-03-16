using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TaxMapManager : MonoBehaviour
{
    public Button stage1;
    public Button stage2;
    public Button stage3;
    public Button stage4;
    public Button stage5;

    async void Start()
    {
        var unlock = await APIManager.Instance.GetStageUnlock();

        int nextStage = unlock.next_stage;

        stage1.interactable = nextStage >= 1;
        stage2.interactable = nextStage >= 2;
        stage3.interactable = nextStage >= 3;
        stage4.interactable = nextStage >= 4;
        stage5.interactable = nextStage >= 5;
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