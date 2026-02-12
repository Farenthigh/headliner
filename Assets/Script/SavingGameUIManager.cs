using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SavingGameUIManager : MonoBehaviour
{
    public static SavingGameUIManager Instance { get; private set; }

    [SerializeField] private TMP_Text BalanceText;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_Text bankNameText;
    [SerializeField] private GameObject bankPanel;
    [SerializeField] private Button depositButton;
    [SerializeField] private Button withdrawButton;
    [SerializeField] private Image roundTimeImage;
    [SerializeField] private TMP_Text roundTimeText;
    [SerializeField] private Image goalBarFill;
    [SerializeField] private TMP_Text goalTextMoney;
    [SerializeField] private TMP_Text roundMonthText;
    [SerializeField] private GameObject victoryIntroUI;
    [SerializeField] private GameObject victoryResultUI;
    [SerializeField] private GameObject defeatIntroUI;
    [SerializeField] private GameObject defeatResultUI;
    // ของshow star
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private GameObject starPriority1;
    [SerializeField] private GameObject starPriority2;
    [SerializeField] private GameObject starPriority3;

    private BankScript bankScript;
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
    private void Start()
    {
        depositButton.onClick.AddListener(OnDepositButton);
        withdrawButton.onClick.AddListener(OnWithdrawButton);
        UpdateGoalBar();
        // UpdateVictory();
        // BGvictory.SetActive(false); //ปิดหน้า victory ไว้ก่อน
    }

    private void Update()
    {
        if (bankScript == null) return;
        bankNameText.text = bankScript.name;
        BalanceText.text = "เหรียญทองคงเหลือ " + bankScript.GetBalance().ToString("F2");
    }

    public void OnOpenBankPanel(BankScript bankScript)
    {
        this.bankScript = bankScript;
        bankPanel.SetActive(true);
    }
    public bool GetBankPanel()
    {
        return bankPanel.activeSelf;
    }
    public void OnCloseBankPanel()
    {
        if (amountInput != null){
            amountInput.text = "" ;
        }
        bankScript = null;
        bankPanel.SetActive(false);
    }
    public void OnDepositButton()
    {
        if (float.TryParse(amountInput.text, out float amount))
        {
            bankScript.Deposit(amount);
            amountInput.text = "";
            UpdateGoalBar();
        }
    }
    public void OnWithdrawButton()
    {
        //TODO: Implement withdraw functionality //eve
        
        if (bankScript == null) return;
        
        if (float.TryParse(amountInput.text, out float amount))
        {
            bankScript.Withdraw(amount);
            amountInput.text = "";
            UpdateGoalBar();
        }
        else
        {
            Debug.LogWarning("Invalid withdraw amount");
        }
    }
  public void UpdateRoundTime(float currentTime, float fullTime)
{
    // วงกลม
    float fill = 1f - (currentTime / fullTime);
    fill = Mathf.Clamp01(fill);
    roundTimeImage.fillAmount = fill;

    // เลข (ถ้ามี)
    float remainingTime = fullTime - currentTime;
    int countdown = Mathf.CeilToInt(remainingTime);
    if (countdown < 0) countdown = 0;
    roundTimeText.text = countdown.ToString();
}


    public void UpdateGoalBar()
{
    float current = SavingGameLogicManager.Instance.GetAllAssets();
    float goal = SavingGameLogicManager.Instance.GetGoalAmount();

    // แถบสีเขียว
    goalBarFill.fillAmount = Mathf.Clamp01(current / goal);

    // ข้อความ 2,000 / 10,000
    goalTextMoney.text = $"{current:N0} / {goal:N0}";
}

   public void UpdateRoundMonth()
    {
    int currentMonth = SavingGameLogicManager.Instance.GetCurrentMonth();
    int goal = SavingGameLogicManager.Instance.GetGoalMonth();
    roundMonthText.text = $"{currentMonth}/{goal}";
    }

    private bool isVictory = false; 
    public void UpdateVictory()
{
    victoryIntroUI.SetActive(false);
    victoryResultUI.SetActive(false);
    // if (isVictory) return; // ถ้าชนะไปแล้ว ไม่ต้องเช็คซ้ำ ***ยังใช้ไม่ได้

    float current = SavingGameLogicManager.Instance.GetAllAssets();
    float goal = SavingGameLogicManager.Instance.GetGoalAmount();

    if (current >= goal)
    {
        isVictory = true;
        StartCoroutine(VictoryFlow()); //ถ้าชนะทำการเปิดลำดับการเรียกหน้าชนะ
    }
}


private bool isDefeat = false;
public void UpdateDefeat()
{
    // ปิดทุกหน้าไว้ก่อน
    defeatIntroUI.SetActive(false);
    defeatResultUI.SetActive(false);

    // if (isDefeat || isVictory) return; 
    // ถ้าชนะไปแล้ว หรือแพ้ไปแล้ว ไม่ต้องเช็คซ้ำ ***ยังใช้ไม่ได้

    int currentMonth = SavingGameLogicManager.Instance.GetCurrentMonth();
    int goalMonth = SavingGameLogicManager.Instance.GetGoalMonth();

    float currentMoney = SavingGameLogicManager.Instance.GetAllAssets();
    float goalMoney = SavingGameLogicManager.Instance.GetGoalAmount();

    // เหตุการแพ้ หมดเดือนแต่เงินยังไม่ถึงเป้า
    if (currentMonth >= goalMonth && currentMoney < goalMoney)
    {
        isDefeat = true;
        StartCoroutine(DefeatFlow());
        Debug.Log("DEFEAT!");
    }
}

// ลำดับการแสดงหน้า UI ของชนะ
private IEnumerator VictoryFlow()
{

    // แสดงหน้า VictoryInto
    victoryIntroUI.SetActive(true);
    Debug.Log("VICTORY!");

    yield return new WaitForSeconds(2.5f); // แสดง 2-3 วิ

    // เปลี่ยนไปหน้าสรุปคะแนน
    victoryIntroUI.SetActive(false);
    ShowStars();
    victoryResultUI.SetActive(true);
    
}

// ลำดับการแสดงหน้า UI ของแพ้
private IEnumerator DefeatFlow()
{
    
    //  แสดงหน้าแพ้ สั้นๆ
    defeatIntroUI.SetActive(true);
    Debug.Log("DEFEAT!");

    yield return new WaitForSeconds(2.5f);

    // เปลี่ยนไปหน้าสรุปคะแนน
    defeatIntroUI.SetActive(false);
    defeatResultUI.SetActive(true);
}

 public void ShowStars()
    {
       
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        starPriority1.SetActive(false);
        starPriority2.SetActive(false);
        starPriority3.SetActive(false);
       
        int starCount = SavingGameLogicManager.Instance.GetStar();
        
        if (starCount >= 1)
        {
            star1.SetActive(true);
            starPriority1.SetActive(true);
        }

        if (starCount >= 2)
        {
            star2.SetActive(true);
            starPriority2.SetActive(true);
        }

        if (starCount >= 3)
        {
            star3.SetActive(true);
            starPriority3.SetActive(true);
        }
    }

}