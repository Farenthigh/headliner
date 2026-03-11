using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankPanelUI : MonoBehaviour
{
    public static BankPanelUI Instance { get; private set; }

    [SerializeField] private TMP_Text BalanceText;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_Text bankNameText;
    [SerializeField] private GameObject bankPanel;
    [SerializeField] private Button depositButton;
    [SerializeField] private Button withdrawButton;
    [SerializeField] private Button closeBankPanelButton;
    [SerializeField] private Transform descriptionParent;
    [SerializeField] private GameObject descriptionText;
    [SerializeField] private TMP_Text FailedText;
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
        closeBankPanelButton.onClick.AddListener(OnCloseBankPanel);
        bankPanel.SetActive(false);
        FailedText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (bankScript == null) return;
        bankNameText.text = bankScript.name;
        BalanceText.text = bankScript.GetBalance().ToString("F2");
    }
    public bool GetBankPanel()
    {
        return bankPanel.activeSelf;
    }
    public void OnOpenBankPanel(BankScript bankScript)
    {
        this.bankScript = bankScript;
        bankPanel.SetActive(true);
        ShowBankDescription(bankScript.GetDescription());
    }
    public void ShowBankDescription(List<string> descriptions)
    {
        // ลบข้อความเก่า
        foreach (Transform child in descriptionParent)
        {
            Destroy(child.gameObject);
        }
        // สร้างข้อความใหม่
        foreach (string desc in descriptions)
        {
            GameObject newTextObj = Instantiate(descriptionText, descriptionParent);
            TMP_Text tmpText = newTextObj.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = desc;
            }
        }
    }
    public void OnCloseBankPanel()
    {
        Debug.Log("Closing Bank Panel");
        if (amountInput != null)
        {
            amountInput.text = "";
        }
        bankScript = null;
        bankPanel.SetActive(false);
    }
    public void OnDepositButton()
    {
        if (!float.TryParse(amountInput.text, out float amount))
        {
            OnFailedAction("กรุณาใส่จำนวนเงินที่ถูกต้อง");
            return;
        }
        if (bankScript.GetMinimumDeposit() > amount)
        {
            OnFailedAction($"ไม่สามารถฝากได้ เนื่องจากยอดฝากขั้นต่ำคือ {bankScript.GetMinimumDeposit()}");
            return;
        }
        bankScript.Deposit(amount);
        amountInput.text = "";
    }
    public void OnWithdrawButton()
    {

        if (bankScript == null) return;

        if (float.TryParse(amountInput.text, out float amount))
        {
            bankScript.Withdraw(amount);
            amountInput.text = "";
            //UpdateGoalBar();
        }
        else
        {
            Debug.LogWarning("Invalid withdraw amount");
        }
    }
    public void OnFailedAction(string message)
    {
        FailedText.text = message;
        FailedText.gameObject.SetActive(true);
    }
}