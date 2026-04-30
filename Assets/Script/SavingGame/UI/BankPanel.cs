using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    [SerializeField] private Image characterImageUI;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueBox;


    private Coroutine typingCoroutine;
    private bool isTyping = false;
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

        // ❗ ของเดิม (อย่าไปยุ่ง)
        ShowBankDescription(bankScript.GetDescription());

        // ✅ เพิ่มตัวละคร
        characterImageUI.sprite = bankScript.GetCharacterImage();

        // ✅ เพิ่ม dialogue 1 ประโยค
        string dialogue = bankScript.GetDialogue();

        if (!string.IsNullOrEmpty(dialogue))
        {
            dialogueBox.SetActive(true);
            StartTyping(dialogue);
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }
    public void OnCloseBankPanel()
    {
        Debug.Log("Closing Bank Panel");
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = "";

        if (amountInput != null)
        {
            amountInput.text = "";
        }

        bankScript = null;
        bankPanel.SetActive(false); ;
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
        if (bankScript.GetMaximumDeposit() < amount)
        {
            OnFailedAction($"ไม่สามารถฝากได้ เนื่องจากยอดฝากขั้นสูงคือ {bankScript.GetMaximumDeposit()}");
            return;
        }
        if (SavingGameLogicManager.Instance.GetCurrentCash() < amount)
        {
            OnFailedAction("ไม่สามารถฝากได้ เนื่องจากคุณมีเงินไม่เพียงพอ");
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
        /* The line `FailedText.gameObject.SetActive(true);` is attempting to set the `gameObject` property of the `FailedText` object to be active, making it visible in the UI. However, it seems that the `FailedText` variable is currently commented out in the code, so this line will result in an error because `FailedText` is not defined or accessible in the current context. */
        FailedText.gameObject.SetActive(true);
    }
    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
    void StartTyping(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(text));
    }
    public void ShowBankDescription(List<string> descriptions)
    {
        // ลบของเก่า
        foreach (Transform child in descriptionParent)
        {
            Destroy(child.gameObject);
        }

        // สร้างใหม่
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