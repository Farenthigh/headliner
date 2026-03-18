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

[Serializable]
public struct UpdateUsernameStruct
{
    public string username;
}

[Serializable]
public struct UpdatePasswordStruct
{
    public string current_password;
    public string new_password;
}

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        client.BaseAddress = new Uri("http://localhost:8080/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Token);
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
        HttpResponseMessage response = await client.GetAsync("users/data/");
        var getResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<ApiResponse<UserData>>(getResponse);
        myData = jsonResponse.data;
        
        return response.Headers.Location;
    }

        public async Task<bool> UpdateUsername(string newUsername)
    {
        try
        {
            UpdateUsernameStruct data = new UpdateUsernameStruct
            {
                username = newUsername
            };

            HttpResponseMessage response = await client.PutAsync(
                "users/updateusername/",
                new StringContent(
                    JsonUtility.ToJson(data),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                myData.username = newUsername;   // ⭐ เพิ่มตรงนี้
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
                current_password = currentPassword,
                new_password = newPassword
            };

            HttpResponseMessage response = await client.PutAsync(
                "users/updatepassword/",
                new StringContent(
                    JsonUtility.ToJson(data),
                    System.Text.Encoding.UTF8,
                    "application/json"));

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
