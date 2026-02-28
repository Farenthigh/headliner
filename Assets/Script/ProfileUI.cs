using TMPro;
using UnityEngine;

public class ProfileUI : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI emailText;
    public TextMeshProUGUI passwordText;

    private void OnEnable()
    {
        ShowUserData();
    }

    void ShowUserData()
    {
        usernameText.text = "Username: " + APIManager.myData.username;
        emailText.text = "Email: " + APIManager.myData.email;

        // ไม่ควรโชว์รหัสผ่านจริง
        passwordText.text = "Password: ********";
    }
}