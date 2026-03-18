using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class ChangePasswordUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField repeatPasswordInput;

    [Header("Pages")]
    public GameObject profilePage;
    public GameObject passwordPage;

    [Header("Panels")]
    public GameObject loadingPanel;

    [Header("Popups")]
    public GameObject successPopup;

    [Header("Error Messages")]
    public GameObject currentPasswordError;
    public GameObject repeatPasswordError;

    bool isProcessing = false;

    void Start()
    {
        // ซ่อน error ตอนเริ่ม
        currentPasswordError.SetActive(false);
        repeatPasswordError.SetActive(false);

        // ซ่อน loading
        loadingPanel.SetActive(false);

        // ซ่อน error เมื่อเริ่มพิมพ์
        currentPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
        newPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
        repeatPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
    }

    public async void SavePassword()
    {
        if (isProcessing)
            return;

        isProcessing = true;

        HideErrors();

        string currentPassword = currentPasswordInput.text;
        string newPassword = newPasswordInput.text;
        string repeatPassword = repeatPasswordInput.text;

        if (string.IsNullOrWhiteSpace(currentPassword) ||
            string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(repeatPassword))
        {
            Debug.Log("Please fill all fields");
            isProcessing = false;
            return;
        }

        if (newPassword != repeatPassword)
        {
            repeatPasswordError.SetActive(true);
            isProcessing = false;
            return;
        }

        SetLoading(true);

        // ให้ Unity render Loading ก่อน
        await Task.Yield();

        bool success = true;

#if UNITY_EDITOR
        Debug.Log("Password updated (TEST MODE)");
#else
        success = await APIManager.Instance.UpdatePassword(
            currentPassword,
            newPassword
        );
#endif

        SetLoading(false);

        if (success)
        {
            currentPasswordInput.text = "";
            newPasswordInput.text = "";
            repeatPasswordInput.text = "";

            successPopup.SetActive(true);
        }
        else
        {
            currentPasswordError.SetActive(true);
        }

        isProcessing = false;
    }

    void SetLoading(bool state)
    {
        loadingPanel.SetActive(state);

        currentPasswordInput.interactable = !state;
        newPasswordInput.interactable = !state;
        repeatPasswordInput.interactable = !state;
    }

    public void GoToProfile()
    {
        successPopup.SetActive(false);
        passwordPage.SetActive(false);
        profilePage.SetActive(true);
    }

    void HideErrors()
    {
        currentPasswordError.SetActive(false);
        repeatPasswordError.SetActive(false);
    }
}