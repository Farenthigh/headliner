using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance; // ตัวแปรนี้ทำให้เราเรียกใช้จากหน้าไหนก็ได้

    [Header("User Data")]
    public string Token = "";       // เก็บตั๋ว Token
    public int UserId = 0;          // เก็บ ID ผู้เล่น
    public string Username = "";    // เก็บชื่อผู้เล่น
    public string Email = "";       // เก็บอีเมล
    public int CharacterId = 0;     // เก็บตัวละครที่เลือก

    void Awake()
    {
        // ระบบ "ตัวอมตะ" ข้าม Scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // สั่งว่า "ห้ามทำลายตอนเปลี่ยนหน้าจอนะ!"
        }
        else
        {
            Destroy(gameObject); // ถ้ามีตัวซ้ำให้ทำลายทิ้ง
        }
    }

    // ฟังก์ชันเช็คว่าล็อกอินอยู่หรือเปล่า
    public bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(Token);
    }

    // ฟังก์ชันล้างข้อมูลตอนกดออกจากระบบ
    public void Logout()
    {
        Token = "";
        UserId = 0;
        Username = "";
        Email = "";
        CharacterId = 0;
    }
}