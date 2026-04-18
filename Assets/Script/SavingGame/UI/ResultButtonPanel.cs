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
        resultButtonPanel.SetActive(true);
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
        SceneManager.LoadScene("SelectGame");
    }
    public void OnPlayAgainButtonClicked()
    {
        SceneManager.LoadScene("SavingGameMap");
    }
    public void OnNextGameButtonClicked()
    {
        SceneManager.LoadScene(scene.name);
    }
}