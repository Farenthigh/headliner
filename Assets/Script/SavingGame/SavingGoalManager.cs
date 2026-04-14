
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SavingGoalManager : MonoBehaviour
{
    public static SavingGoalManager Instance { get; private set; }
    [SerializeField] private int stage;
    [SerializeField] private float baseGoal = 1000f;
    [SerializeField] private float goalAmount2 = 5000f;
    [SerializeField] private float goalAmount3 = 10000f;
    [SerializeField] private int goalMonth = 12;
    [SerializeField] private Image StarGoal1;
    [SerializeField] private Image StarGoal2;
    [SerializeField] private Image StarGoal3;
    [SerializeField] private string Priority1;
    [SerializeField] private GameObject PriorityParent1;
    [SerializeField] private TMP_Text PriorityText1;
    [SerializeField] private Image PriorityImage1;
    [SerializeField] private string Priority2;
    [SerializeField] private GameObject PriorityParent2;
    [SerializeField] private TMP_Text PriorityText2;
    [SerializeField] private Image PriorityImage2;
    [SerializeField] private string Priority3;
    [SerializeField] private GameObject PriorityParent3;
    [SerializeField] private TMP_Text PriorityText3;
    [SerializeField] private Image PriorityImage3;
    [SerializeField] private Image ResultBackground1;
    [SerializeField] private Image ResultBackground2;
    [SerializeField] private Sprite defeatBackground1;
    [SerializeField] private Sprite defeatBackground2;
    [SerializeField] private Sprite passStar;
    [SerializeField] private Button exit;
    [SerializeField] private Button playAgain;
    [SerializeField] private Button nextGame;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PriorityText1.text = Priority1;
        PriorityText2.text = Priority2;
        PriorityText3.text = Priority3;
        ResultBackground1.gameObject.SetActive(false);
        ResultBackground2.gameObject.SetActive(false);
        PriorityParent1.SetActive(false);
        PriorityParent2.SetActive(false);
        PriorityParent3.SetActive(false);

        exit.onClick.AddListener(OnExitClick);
        playAgain.onClick.AddListener(OnPlayAgainClick);
        nextGame.onClick.AddListener(OnNextGameClick);

        if (stage == 5)
        {
            nextGame.gameObject.SetActive(false);
        }

    }
    public void CheckGoals(float totalAssets)
    {

        // หยุดเวลาเมื่อเลยเดือนเป้าหมายแล้ว
        // ปิดทุกหน้าต่างที่เกี่ยวข้องกับการเล่นเกม
        // แสดงPanelที่บอกว่าเกมจบแล้ว

        int starsEarned = 0;

        if (totalAssets >= goalAmount3)
        {
            Debug.Log("Congratulations! You've reached Goal 3!");
            starsEarned += 1;
            PriorityImage3.sprite = passStar;
        }
        if (totalAssets >= goalAmount2)
        {
            Debug.Log("Great job! You've reached Goal 2!");
            starsEarned += 1;
            PriorityImage2.sprite = passStar;
        }
        if (totalAssets >= baseGoal)
        {
            Debug.Log("Good start! You've reached Goal 1!");
            starsEarned += 1;
            PriorityImage1.sprite = passStar;
        }
        if (starsEarned == 0)
        {
            ResultBackground1.sprite = defeatBackground1;
            ResultBackground2.sprite = defeatBackground2;
        }
        else
        {
            if (starsEarned >= 1) StarGoal1.sprite = passStar;
            if (starsEarned >= 2) StarGoal2.sprite = passStar;
            if (starsEarned >= 3) StarGoal3.sprite = passStar;
        }
        Debug.Log($"Total Assets: {totalAssets}, Stars Earned: {starsEarned}");
        StartCoroutine(ShowResults());
        try
        {
            if (starsEarned > 0)
            {
                APIManager.Instance.SavingGameResult(stage, starsEarned);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to send saving game result: " + ex.Message);
        }
    }

    public float GetGoalAmount()
    {
        return baseGoal;
    }
    public int GetGoalMonth()
    {
        return goalMonth;
    }
    public void AddGoal(float goal)
    {

        baseGoal += goal;
    }
    private IEnumerator ShowResults()
    {
        ResultBackground1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        ResultBackground1.gameObject.SetActive(false);
        ResultBackground2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        PriorityParent1.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PriorityParent2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PriorityParent3.SetActive(true);
        yield return new WaitForSeconds(1f);
        ResultButtonPanelUI.Instance.SetShowPanel(true);
    }
    private void OnExitClick()
    {
        SceneManager.LoadScene("SavingGameStageInfo");
    }
    private void OnPlayAgainClick()
    {
        SceneManager.LoadScene($"SavingGameStage{stage}");
    }
    private void OnNextGameClick()
    {
        SceneManager.LoadScene($"SavingGameStage{stage + 1}");
    }
}