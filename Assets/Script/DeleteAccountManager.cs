using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteAccountManager : MonoBehaviour
{
    public GameObject deleteConfirmPanel;

    public void ConfirmDelete()
    {
        // 🔴 ลบข้อมูลผู้ใช้
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.DeleteKey("Password");
        PlayerPrefs.Save();

        // 🔴 ปิด popup
        deleteConfirmPanel.SetActive(false);

        // 🔴 กลับไปหน้า Login
        SceneManager.LoadScene("LoginScene");
    }
}