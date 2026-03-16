using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class ChangeUsernameUI : MonoBehaviour
{
    public TMP_InputField newUsernameInput;
    public TMP_Text errorText;
    public GameObject changeUsernamePage;
    public GameObject profilePage;
    public GameObject successPopup;

    private static readonly Regex usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");

    private void Start()
    {
        // ล้าง error เมื่อผู้เล่นเริ่มพิมพ์
        newUsernameInput.onValueChanged.AddListener(OnUsernameChanged);
    }

    public async void SaveUsername()
    {
        errorText.gameObject.SetActive(false);

        string newName = newUsernameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            ShowError("Username cannot be empty");
            return;
        }

        if (newName.Length > 20)
        {
            ShowError("Username must be less than 20 characters");
            return;
        }

        if (!usernameRegex.IsMatch(newName))
        {
            ShowError("Username can only contain letters, numbers, and _");
            return;
        }

        if (APIManager.Instance == null)
        {
            Debug.LogError("APIManager not found");
            ShowError("System error");
            return;
        }

#if UNITY_EDITOR
        APIManager.myData.username = newName;
        successPopup.SetActive(true);
#else
        bool success = await APIManager.Instance.UpdateUsername(newName);

        if (!success)
        {
            ShowError("Username already taken");
            return;
        }

        await APIManager.Instance.GetMyData();

        successPopup.SetActive(true);
#endif
    }

    void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    // ฟังก์ชันล้าง error ตอนพิมพ์
    void OnUsernameChanged(string value)
    {
        errorText.gameObject.SetActive(false);
    }

    public void GoToProfile()
    {
        successPopup.SetActive(false);
        changeUsernamePage.SetActive(false);
        profilePage.SetActive(true);
    }
}