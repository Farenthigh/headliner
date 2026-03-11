using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Button closeBankPanelButton;
    [SerializeField] private Transform descriptionParent;
    [SerializeField] private GameObject descriptionText;
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
    private void Update()
    {
        if (bankScript == null) return;
        bankNameText.text = bankScript.name;
        BalanceText.text = bankScript.GetBalance().ToString("F2");

    }


    // public void OnOpenBankPanel(BankScript bankScript)
    // {
    //     this.bankScript = bankScript;
    //     bankPanel.SetActive(true);
    //     ShowBankDescription(bankScript.GetDescription());
    // }
    // public bool GetBankPanel()
    // {
    //     return bankPanel.activeSelf;
    // }
    // public void OnCloseBankPanel()
    // {
    //     Debug.Log("Closing Bank Panel");
    //     if (amountInput != null)
    //     {
    //         amountInput.text = "";
    //     }
    //     bankScript = null;
    //     bankPanel.SetActive(false);
    // }
    // public void OnDepositButton()
    // {
    //     if (!float.TryParse(amountInput.text, out float amount))
    //     {
    //         Debug.LogWarning("Invalid deposit amount");
    //         return;
    //     }
    //     if (bankScript.GetMinimumDeposit() > amount)
    //     {
    //         Debug.LogWarning($"Deposit amount must be at least {bankScript.GetMinimumDeposit()}");
    //         return;
    //     }
    //     bankScript.Deposit(amount);
    //     amountInput.text = "";
    // }
    // public void OnWithdrawButton()
    // {
    //     //TODO: Implement withdraw functionality //eve

    //     if (bankScript == null) return;

    //     if (float.TryParse(amountInput.text, out float amount))
    //     {
    //         bankScript.Withdraw(amount);
    //         amountInput.text = "";
    //         //UpdateGoalBar();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Invalid withdraw amount");
    //     }
    // }
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

    public void UpdateRoundMonth()
    {
        int currentMonth = SavingGameLogicManager.Instance.GetCurrentMonth();
        int goal = SavingGameLogicManager.Instance.GetGoalMonth();
        roundMonthText.text = $"{currentMonth}/{goal}";
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

}