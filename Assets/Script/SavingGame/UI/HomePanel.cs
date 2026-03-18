using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    public static HomePanel Instance { get; private set; }

    [SerializeField] private GameObject homePanel;
    [SerializeField] private Button closeHomePanel;
    [SerializeField] private Transform descriptionParent;
    [SerializeField] private GameObject descriptionText;

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
        homePanel.SetActive(false);
        closeHomePanel.onClick.AddListener(CloseHomePanel);
    }
    public bool GetHomePanel()
    {
        return homePanel.activeSelf;
    }
    public void CloseHomePanel()
    {
        homePanel.SetActive(false);
    }
    private void ShowHomeDescription()
    {
        // ลบข้อความเก่า
        foreach (Transform child in descriptionParent)
        {
            Destroy(child.gameObject);
        }
        // สร้างข้อความใหม่
        foreach (BankScript bank in SavingGameLogicManager.Instance.banks)
        {
            GameObject newTextObj = Instantiate(descriptionText, descriptionParent);
            TMP_Text tmpText = newTextObj.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = $"{bank.name}: {bank.GetBalance()}";
            }
        }
        GameObject newTextCashObj = Instantiate(descriptionText, descriptionParent);
        TMP_Text tmpTextCash = newTextCashObj.GetComponent<TMP_Text>();
        if (tmpTextCash != null)
        {
            tmpTextCash.text = $"Cash: {SavingGameLogicManager.Instance.GetCurrentCash()}";
        }
    }
    public void OnOpenHomePanel()
    {
        ShowHomeDescription();
        homePanel.SetActive(true);
        PieChart.Instance.GetAllAssets();
    }
}