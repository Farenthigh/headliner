using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoutController : MonoBehaviour
{
    [SerializeField] private Button logoutButton;

    private void Start()
    {
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(HandleLogout);
        }
    }

    private void HandleLogout()
    {
        // 1. สั่งให้ APIManager ล้างข้อมูลเก่าทิ้งให้หมด
        if (APIManager.Instance != null)
        {
            APIManager.Instance.Logout();
        }

        // 2. โหลดกลับไปหน้า Login
        // (*** อย่าลืมเปลี่ยนคำว่า "LoginScene" เป็นชื่อหน้าล็อกอินของคุณจริงๆ นะครับ ***)
        SceneManager.LoadScene("LoginScene"); 
    }
}