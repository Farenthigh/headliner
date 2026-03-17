using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class login : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [Header("Password Visibility Toggle")]
    [SerializeField] private Button toggleEyeButton;
    [SerializeField] private Image eyeIconImage;
    [SerializeField] private Sprite eyeOpenSprite;
    [SerializeField] private Sprite eyeClosedSprite;

    private bool isPasswordVisible = false;

    private void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
        registerButton.onClick.AddListener(HandleRegister);

        if (toggleEyeButton != null)
        {
            toggleEyeButton.onClick.AddListener(TogglePasswordVisibility);
        }

        passwordInput.contentType = TMP_InputField.ContentType.Password;
        if (eyeIconImage != null && eyeClosedSprite != null) 
            eyeIconImage.sprite = eyeClosedSprite;

    }
    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible; 

        if (isPasswordVisible)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            if (eyeIconImage != null && eyeOpenSprite != null) 
                eyeIconImage.sprite = eyeOpenSprite;
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            if (eyeIconImage != null && eyeClosedSprite != null) 
                eyeIconImage.sprite = eyeClosedSprite;
        }

        passwordInput.ForceLabelUpdate();
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
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
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
