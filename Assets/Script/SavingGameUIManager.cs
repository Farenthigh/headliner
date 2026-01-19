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
        bankPanel.SetActive(false);
    }
    public void OnDepositButton()
    {
        if (float.TryParse(amountInput.text, out float amount))
        {
            bankScript.Deposit(amount);
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
        }
        else
        {
            Debug.LogWarning("Invalid withdraw amount");
        }
    }
    public void UpdateRoundTime()
    {
        //TODO: Time in one round - fulltimeinseconds //kf
        //Update call UpdateRoundtime
    }
    public void UpdateGoalBar()
    {
        //TODO: create serializafield in logicmanager for goal amount //kf
        //create function GetAllAssets in logicmanager //kf
    }
}