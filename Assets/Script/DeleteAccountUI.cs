using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeleteAccountUI : MonoBehaviour
{
    [Header("Popups")]
    public GameObject confirmPopup;   // popup ซ้าย
    public GameObject passwordPopup;  // popup ขวา

    [Header("Password")]
    public TMP_InputField passwordInput;
    public TMP_Text errorText;

    // กด Delete Account ที่หน้า Profile
    public void OpenConfirmPopup()
    {
        confirmPopup.SetActive(true);
    }

    // Cancel popup ซ้าย
    public void CancelConfirm()
    {
        confirmPopup.SetActive(false);
    }

    // Delete popup ซ้าย → ไป popup ใส่รหัส
    public void GoToPasswordPopup()
    {
        confirmPopup.SetActive(false);
        passwordPopup.SetActive(true);
    }

    // Cancel popup ขวา
    public void CancelPassword()
    {
        passwordPopup.SetActive(false);
    }

    public async void DeleteAccount()
    {
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Please enter password";
            errorText.gameObject.SetActive(true);
            return;
        }

        // #if UNITY_EDITOR
        //         Debug.Log("Delete Account (Test Mode)");
        // #else
        //         bool success = await APIManager.Instance.DeleteAccount(passwordInput.text);

        //         if (!success)
        //         {
        //             errorText.text = "Password is incorrect";
        //             errorText.gameObject.SetActive(true);
        //             return;
        //         }
        // #endif

        //         Debug.Log("Account Deleted");

        //         passwordPopup.SetActive(false);

        //         LoadingManager.Instance.LoadScene("LoginScene");
    }
}