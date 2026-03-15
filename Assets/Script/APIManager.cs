using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct ApiResponse<T>
{
    // JsonUtility requires public FIELDS, not properties
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


public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
    public static string Token;
    public static UserData myData;
    static HttpClient client = new HttpClient();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Token);
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
        // +++ 1. แนบตั๋ว VIP (Token) ก่อนส่ง Request ทุกครั้ง +++
        client.DefaultRequestHeaders.Remove("Authorization");
        Debug.Log("🔑 ตั๋ว Token ที่มีตอนนี้คือ: [" + Token + "]"); // ล้างของเก่ากันเหนียว
        if (!string.IsNullOrEmpty(Token)) 
        {
            // +++ 1. สั่งตัดช่องว่าง และเครื่องหมายคำพูด " ที่อาจจะแอบซ่อนอยู่ออกให้เกลี้ยง! +++
            string cleanToken = Token.Trim().Replace("\"", ""); 
            
            // +++ 2. แนบตั๋วที่สะอาดแล้วเข้าไป +++
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + cleanToken);
        }

        HttpResponseMessage response = await client.GetAsync("users/data/");
        var getResponse = await response.Content.ReadAsStringAsync();

        // +++ 2. เช็คก่อนว่า Backend ตอบกลับมาสำเร็จไหม (200 OK) ก่อนที่จะพยายามแกะ JSON +++
        if (!response.IsSuccessStatusCode)
        {
            // ถ้าพัง ให้ปริ้นท์ออกมาดูเลยว่า Backend บ่นอะไร จะได้แก้ถูกจุด!
            Debug.LogError($"ดึงข้อมูลล้มเหลว! Status: {response.StatusCode} | ข้อความ: {getResponse}");
            
            // สั่งโยน Error กลับไปให้ catch ในหน้า Login ทำงาน
            response.EnsureSuccessStatusCode(); 
        }

        // 3. ถ้าสำเร็จ ค่อยเอาข้อความมาแกะเป็น JSON อย่างปลอดภัย
        var jsonResponse = JsonUtility.FromJson<ApiResponse<UserData>>(getResponse);
        myData = jsonResponse.data;
        
        return response.Headers.Location;
    }
    public void Logout()
    {
        // 1. ล้างข้อมูลตัวแปรของคนเก่า
        Token = "";
        myData = new UserData();

        // +++ 2. ล้างสมอง HttpClient! ลบ Header และคราบสกปรกเก่าๆ ทิ้งให้หมดเกลี้ยง! +++
        client.DefaultRequestHeaders.Clear();
        
        // (ถ้าเกมคุณจำเป็นต้องบอก Backend ว่าขอรับข้อมูลเป็น JSON ให้ใส่บรรทัดล่างนี้เผื่อไว้ด้วยครับ)
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        Debug.Log("ออกจากระบบ และล้างความทรงจำ HttpClient เรียบร้อย!");
    }
}
