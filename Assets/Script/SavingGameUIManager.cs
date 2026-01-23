using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

}