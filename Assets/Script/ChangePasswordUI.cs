using UnityEngine;
using TMPro;

public class ChangePasswordUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField repeatPasswordInput;

    [Header("Pages")]
    public GameObject profilePage;
    public GameObject passwordPage;

    [Header("Popups")]
    public GameObject successPopup;

    [Header("Error Messages")]
    public GameObject currentPasswordError;
    public GameObject repeatPasswordError;

    void Start()
    {
        // ซ่อน error ตอนเริ่ม
        currentPasswordError.SetActive(false);
        repeatPasswordError.SetActive(false);

        // ให้ error หายเมื่อเริ่มพิมพ์
        currentPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
        newPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
        repeatPasswordInput.onValueChanged.AddListener(delegate { HideErrors(); });
    }

    public async void SavePassword()
    {
        // ซ่อน error ก่อนตรวจ
        currentPasswordError.SetActive(false);
        repeatPasswordError.SetActive(false);

        if (string.IsNullOrEmpty(currentPasswordInput.text) ||
            string.IsNullOrEmpty(newPasswordInput.text) ||
            string.IsNullOrEmpty(repeatPasswordInput.text))
        {
            Debug.Log("Please fill all fields");
            return;
        }

        // 🔴 ตรวจ repeat password
        if (newPasswordInput.text != repeatPasswordInput.text)
        {
            repeatPasswordError.SetActive(true);
            return;
        }

        bool success = true;

#if UNITY_EDITOR
        Debug.Log("Password updated (TEST MODE)");
#else
        success = await APIManager.Instance.UpdatePassword(
            currentPasswordInput.text,
            newPasswordInput.text
        );
#endif

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