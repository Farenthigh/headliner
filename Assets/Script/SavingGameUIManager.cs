using TMPro;
using UnityEngine;

public class SavingGameUIManager : MonoBehaviour
{
    public static SavingGameUIManager Instance { get; private set; }

    [SerializeField] private TMP_Text BalanceText;
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_Text bankNameText;
    [SerializeField] private GameObject bankPanel;

    private int balance = 0;
    private string bankName = "Default Bank";
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
        bankNameText.text = bankName;
        BalanceText.text = "เหรียญทองคงเหลือ " + balance.ToString("F2");
    }

    public void OnOpenBankPanel(string bankName)
    {
        this.bankName = bankName;
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
}