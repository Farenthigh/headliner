using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class login : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        loginButton.onClick.AddListener(HandleLogin);
    }
    private void HandleLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log($"Attempting login with Username: {username} and Password: {password}");
    }
}
