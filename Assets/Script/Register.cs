using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Register : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmpasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        registerButton.onClick.AddListener(HandleRegister);
        loginButton.onClick.AddListener(HandleLogin);
    }
    private void HandleLogin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    private async void HandleRegister()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmpass = confirmpasswordInput.text;

        if (password != confirmpass)
        {
            Debug.Log("Passwords do not match");
            return;
        }
        try
        {
            await APIManager.Instance.Register(new RegisterStruct
            {
                email = email,
                password = password,
                confirm_password = confirmpass
            });

            UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
        }
        catch (System.Exception ex)
        {
            Debug.Log("Registration failed: " + ex.Message);
        }

    }

}
