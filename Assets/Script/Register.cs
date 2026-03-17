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
    [SerializeField] private Sprite eyeOpenSprite;
    [SerializeField] private Sprite eyeClosedSprite;
    [SerializeField] private Button toggleEyeButton1;
    [SerializeField] private Image eyeIconImage1;
    private bool isPasswordVisible1 = false;
    [SerializeField] private Button toggleEyeButton2;
    [SerializeField] private Image eyeIconImage2;
    private bool isPasswordVisible2 = false;

    private bool isPasswordVisible = false;

    private void Start()
    {
        registerButton.onClick.AddListener(HandleRegister);
        loginButton.onClick.AddListener(HandleLogin);

        if (toggleEyeButton1 != null) toggleEyeButton1.onClick.AddListener(TogglePassword1);
        if (toggleEyeButton2 != null) toggleEyeButton2.onClick.AddListener(TogglePassword2);

        passwordInput.contentType = TMP_InputField.ContentType.Password;
        if (eyeIconImage1 != null && eyeClosedSprite != null) eyeIconImage1.sprite = eyeClosedSprite;

        confirmpasswordInput.contentType = TMP_InputField.ContentType.Password;
        if (eyeIconImage2 != null && eyeClosedSprite != null) eyeIconImage2.sprite = eyeClosedSprite;
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

    private void TogglePassword1()
    {
        isPasswordVisible1 = !isPasswordVisible1; 
        UpdateVisibility(passwordInput, eyeIconImage1, isPasswordVisible1);
    }

    private void TogglePassword2()
    {
        isPasswordVisible2 = !isPasswordVisible2; 
        UpdateVisibility(confirmpasswordInput, eyeIconImage2, isPasswordVisible2);
    }

    private void UpdateVisibility(TMP_InputField inputField, Image icon, bool isVisible)
    {
        if (isVisible)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard; 
            if (icon != null && eyeOpenSprite != null) icon.sprite = eyeOpenSprite;
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
            if (icon != null && eyeClosedSprite != null) icon.sprite = eyeClosedSprite;
        }

        inputField.ForceLabelUpdate();
    }

}
