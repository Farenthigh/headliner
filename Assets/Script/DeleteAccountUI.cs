using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DeleteAccountUI : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public GameObject deletePanel;

    public async void DeleteAccount()
    {
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.Log("Please enter password");
            return;
        }

#if UNITY_EDITOR
        Debug.Log("Delete Account (Test Mode)");
#else
        bool success = await APIManager.Instance.DeleteAccount(passwordInput.text);

        if (!success)
        {
            Debug.Log("Delete failed");
            return;
        }
#endif

        Debug.Log("Account Deleted");

        deletePanel.SetActive(false);

        // ⭐ ไปหน้า Login
        SceneManager.LoadScene("LoginScene");
    }

    public void Cancel()
    {
        deletePanel.SetActive(false);
    }
}