using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditProfileUI : MonoBehaviour
{
    [Header("Username")]
    public TMP_InputField usernameInput;
    public TMP_Text usernameError;

    [Header("Password")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;

    public TMP_Text currentPasswordError;
    public TMP_Text confirmPasswordError;

    [Header("Show/Hide Password Buttons")]
    public Button currentEyeButton;
    public Button newEyeButton;
    public Button confirmEyeButton;

    [Header("Success Popup")]
    public GameObject successPopup;

    private string oldUsername;

    void Start()
    {
        oldUsername = APIManager.myData.username;
        usernameInput.text = oldUsername;

        usernameError.gameObject.SetActive(false);
        currentPasswordError.gameObject.SetActive(false);
        confirmPasswordError.gameObject.SetActive(false);

        currentPasswordInput.contentType = TMP_InputField.ContentType.Password;
        newPasswordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
    }

    // =========================
    // SHOW / HIDE PASSWORD
    // =========================

    public void ToggleCurrentPassword()
    {
        TogglePassword(currentPasswordInput);
    }

    public void ToggleNewPassword()
    {
        TogglePassword(newPasswordInput);
    }

    public void ToggleConfirmPassword()
    {
        TogglePassword(confirmPasswordInput);
    }

    void TogglePassword(TMP_InputField field)
    {
        if (field.contentType == TMP_InputField.ContentType.Password)
            field.contentType = TMP_InputField.ContentType.Standard;
        else
            field.contentType = TMP_InputField.ContentType.Password;

        field.ForceLabelUpdate();
    }

    // =========================
    // SAVE CHANGES
    // =========================

    public async void SaveChanges()
    {
        usernameError.gameObject.SetActive(false);
        currentPasswordError.gameObject.SetActive(false);
        confirmPasswordError.gameObject.SetActive(false);

        bool usernameChanged = usernameInput.text != oldUsername;
        bool passwordChanged = newPasswordInput.text != "";

        // ---------- UPDATE USERNAME ----------
        if (usernameChanged)
        {
            bool success = await APIManager.Instance.UpdateUsername(usernameInput.text);

            if (!success)
            {
                usernameError.text = "Username already taken";
                usernameError.gameObject.SetActive(true);
                return;
            }
        }

        // ---------- UPDATE PASSWORD ----------
        if (passwordChanged)
        {
            if (newPasswordInput.text != confirmPasswordInput.text)
            {
                confirmPasswordError.text = "Passwords do not match";
                confirmPasswordError.gameObject.SetActive(true);
                return;
            }

            bool success = await APIManager.Instance.UpdatePassword(
                currentPasswordInput.text,
                newPasswordInput.text
            );

            if (!success)
            {
                currentPasswordError.text = "Incorrect current password";
                currentPasswordError.gameObject.SetActive(true);
                return;
            }
        }

        await APIManager.Instance.GetMyData();

        successPopup.SetActive(true);
    }

    // =========================
    // CLOSE POPUP
    // =========================

    public void CloseSuccessPopup()
    {
        successPopup.SetActive(false);
        gameObject.SetActive(false);
    }

    // =========================
    // CANCEL BUTTON
    // =========================

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}