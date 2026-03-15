
using UnityEngine;
using UnityEngine.UI;

public class SavingGoalManager : MonoBehaviour
{
    public static SavingGoalManager Instance { get; private set; }
    [SerializeField] private float baseGoal = 1000f;
    [SerializeField] private float goalAmount2 = 5000f;
    [SerializeField] private float goalAmount3 = 10000f;
    [SerializeField] private int goalMonth = 12;
    [SerializeField] private Image StarGoal1;
    [SerializeField] private Image StarGoal2;
    [SerializeField] private Image StarGoal3;
    [SerializeField] private Sprite passStar;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        CheckGoals(SavingGameLogicManager.Instance.GetAllAssets());
    }
    public void CheckGoals(float totalAssets)
    {
        if (SavingGameLogicManager.Instance.GetCurrentMonth() >= goalMonth + 1) return;
        // หยุดเวลาเมื่อเลยเดือนเป้าหมายแล้ว
        // ปิดทุกหน้าต่างที่เกี่ยวข้องกับการเล่นเกม
        // แสดงPanelที่บอกว่าเกมจบแล้ว
        if (totalAssets >= goalAmount3)
        {
            Debug.Log("Congratulations! You've reached Goal 3!");
            StarGoal3.sprite = passStar;
        }
        if (totalAssets >= goalAmount2)
        {
            Debug.Log("Great job! You've reached Goal 2!");
            StarGoal2.sprite = passStar;
        }
        if (totalAssets >= baseGoal)
        {
            Debug.Log("Good start! You've reached Goal 1!");
            StarGoal1.sprite = passStar;
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
}