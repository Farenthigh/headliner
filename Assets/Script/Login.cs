using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class login : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
        registerButton.onClick.AddListener(HandleRegister);
    }
    private async void HandleLogin()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        try
        {
            await APIManager.Instance.Login(email, password);
            await APIManager.Instance.GetMyData();
            if (APIManager.myData.character == 0 && APIManager.myData.username == "")
                UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCharacterScene");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("SelectGame");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Login failed: " + ex.Message);
        }
    }

    private void HandleRegister()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RegisterScene");
    }
}
