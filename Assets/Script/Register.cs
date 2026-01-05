using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Register : MonoBehaviour{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmpasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    private void Start(){
        registerButton.onClick.AddListener(HandleRegister);
    }

    private void HandleRegister(){
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmpass = confirmpasswordInput.text;

        Debug.Log($"Register with email: {email} and password: {password}");
    }

}
