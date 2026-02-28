using UnityEngine;
using TMPro;

public class ChangePasswordUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField repeatPasswordInput;

    public GameObject profilePage;   // หน้า Profile
    public GameObject passwordPage;  // หน้า Change Password

    public async void SavePassword()
    {
        // 🔹 เช็คว่ากรอกครบไหม
        if (string.IsNullOrEmpty(currentPasswordInput.text) ||
            string.IsNullOrEmpty(newPasswordInput.text) ||
            string.IsNullOrEmpty(repeatPasswordInput.text))
        {
            Debug.Log("Please fill all fields");
            return;
        }

        // 🔹 เช็ครหัสใหม่ตรงกันไหม
        if (newPasswordInput.text != repeatPasswordInput.text)
        {
            Debug.Log("Password not match");
            return;
        }

        // 🔹 เรียก API
        bool success = await APIManager.Instance.UpdatePassword(
            currentPasswordInput.text,
            newPasswordInput.text
        );

        if (success)
        {
            Debug.Log("Password updated successfully");

            // เคลียร์ช่องกรอก
            currentPasswordInput.text = "";
            newPasswordInput.text = "";
            repeatPasswordInput.text = "";

            // กลับหน้า Profile
            passwordPage.SetActive(false);
            profilePage.SetActive(true);
        }
        else
        {
            Debug.Log("Update failed");
        }
    }
}