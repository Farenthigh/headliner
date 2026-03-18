using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct ApiResponse<T>
{
    public T data;
    public string error;
    public string message;
}

[Serializable]
public struct TokenData
{
    public string token;
}

[Serializable]
public struct UserData
{
    public int id;
    public string email;
    public string username;
    public int character;
}

public struct RegisterStruct
{
    public string email;
    public string password;
    public string confirm_password;
}

public struct LoginStruct
{
    public string email;
    public string password;
}

public struct CharacterStruct
{
    public string username;
    public int character;
}

// +++ โครงสร้างข้อมูลใหม่จากฝั่ง Incoming +++
[Serializable]
public struct UpdateUsernameStruct
{
    public string username;
}

[Serializable]
public struct UpdatePasswordStruct
{
    public string currentPassword;
    public string newPassword;
}

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
    
    // +++ ตัวแปรใหม่จากฝั่ง Incoming +++
    public static bool IsRequestRunning = false; 
    
    public static string Token;
    public static UserData myData;
    static HttpClient client = new HttpClient();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ตั้งค่า Client ตั้งแต่เริ่มเลย จะได้พร้อมใช้เสมอ
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<Uri> Register(RegisterStruct register)
    {
        HttpResponseMessage response = await client.PostAsync(
            "users/register/", new StringContent(
                JsonUtility.ToJson(register),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();

        return response.Headers.Location;
    }

    public async Task<Uri> Login(string email, string password)
    {
        HttpResponseMessage response = await client.PostAsync(
            "users/login/", new StringContent(
                JsonUtility.ToJson(new LoginStruct { email = email, password = password }),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();
        var postResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<ApiResponse<TokenData>>(postResponse);
        Token = jsonResponse.data.token;
        
        // แนบ Bearer ให้ถูกต้อง (ใช้ตามแบบไฟล์ของคุณ ปลอดภัยกว่า)
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        return response.Headers.Location;
    }

    public async Task<Uri> ChooseCharacter(int character, string name)
    {
        Debug.Log("Choosing character " + character + " with name " + name);
        HttpResponseMessage response = await client.PostAsync(
            "users/createcharacter/", new StringContent(
                JsonUtility.ToJson(new CharacterStruct { username = name, character = character }),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();
        return response.Headers.Location;
    }

    public async Task<Uri> GetMyData()
    {
        // +++ ระบบล้าง Token และดัก Error ชั้นเยี่ยมจากไฟล์ของคุณ +++
        client.DefaultRequestHeaders.Remove("Authorization");
        Debug.Log("🔑 ตั๋ว Token ที่มีตอนนี้คือ: [" + Token + "]"); 
        
        if (!string.IsNullOrEmpty(Token)) 
        {
            string cleanToken = Token.Trim().Replace("\"", ""); 
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + cleanToken);
        }

        HttpResponseMessage response = await client.GetAsync("users/data/");
        var getResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"ดึงข้อมูลล้มเหลว! Status: {response.StatusCode} | ข้อความ: {getResponse}");
            response.EnsureSuccessStatusCode(); 
        }

        var jsonResponse = JsonUtility.FromJson<ApiResponse<UserData>>(getResponse);
        myData = jsonResponse.data;
        
        return response.Headers.Location;
    }

    public void Logout()
    {
        // +++ ระบบล้างความทรงจำจากไฟล์ของคุณ +++
        Token = "";
        myData = new UserData();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        Debug.Log("ออกจากระบบ และล้างความทรงจำ HttpClient เรียบร้อย!");
    }

    // ==========================================================
    // +++ ฟังก์ชันใหม่ 3 ตัวที่เพิ่มมาจาก Incoming (ระบบจัดการบัญชี) +++
    // ==========================================================

    public async Task<bool> UpdateUsername(string newUsername)
    {
        try
        {
            UpdateUsernameStruct data = new UpdateUsernameStruct
            {
                username = newUsername
            };

            HttpResponseMessage response = await client.PutAsync(
                "users/update-username/",
                new StringContent(
                    JsonUtility.ToJson(data),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                myData.username = newUsername;   // อัปเดตข้อมูลตัวละครในเครื่องทันที
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Debug.Log("Update username error: " + e.Message);
            return false;
        }
    }

    public async Task<bool> UpdatePassword(string currentPassword, string newPassword)
    {
        try
        {
            UpdatePasswordStruct data = new UpdatePasswordStruct
            {
                currentPassword = currentPassword,
                newPassword = newPassword
            };

            HttpResponseMessage response = await client.PutAsync(
                "users/update-password/",
                new StringContent(
                    JsonUtility.ToJson(data),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                Debug.LogError($"❌ เปลี่ยนรหัสพัง! Status: {response.StatusCode} | Backend ด่ามาว่า: {errorResponse}");
                return false;
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Debug.Log("Update password error: " + e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAccount()
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync("users/delete/");
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            Debug.Log("Delete account error: " + e.Message);
            return false;
        }
    }
}