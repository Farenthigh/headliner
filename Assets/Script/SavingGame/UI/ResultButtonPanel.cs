using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultButtonPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject resultButtonPanel;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button PlayAgainButton;
    [SerializeField] private Button NextGameButton;
    [SerializeField] private Scene scene;
    public static ResultButtonPanelUI Instance;

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
        resultButtonPanel.SetActive(false);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
        PlayAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
        NextGameButton.onClick.AddListener(OnNextGameButtonClicked);
    }

    public void SetShowPanel(bool show)
    {
        resultButtonPanel.SetActive(show);
    }

    public void OnExitButtonClicked()
    {
        LoadingManager.Instance.LoadScene("SelectGame");
    }
    public void OnPlayAgainButtonClicked()
    {
        LoadingManager.Instance.LoadScene("SavingGameMap");
    }
    public void OnNextGameButtonClicked()
    {
        LoadingManager.Instance.LoadScene(scene.name);
    }
}