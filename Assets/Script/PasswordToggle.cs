using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public Image eyeIcon;

    public Sprite eyeOpen;
    public Sprite eyeClosed;

    bool isHidden = true;

    public void TogglePassword()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            eyeIcon.sprite = eyeClosed;
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            eyeIcon.sprite = eyeOpen;
        }

        passwordInput.ForceLabelUpdate();
    }
}