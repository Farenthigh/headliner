using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic; // +++ เพิ่มเข้ามาเพื่อให้ใช้ List<> ของเพื่อนได้ +++
using UnityEngine;

// ==========================================
// 1. โครงสร้างข้อมูลพื้นฐาน (Base Structs)
// ==========================================
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
    public string chatbot_name;
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

// ==========================================
// 2. โครงสร้างข้อมูลจัดการบัญชี (Profile Structs)
// ==========================================
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

// ==========================================
// 3. โครงสร้างข้อมูลด่านและเกม (Stage Structs - จากเพื่อน)
// ==========================================
[Serializable]
public struct GameResultStruct
{
    public int user_id;
    public int stars;
    public int stage;
}

[Serializable]
public struct StageUnlockData
{
    public int now_stage;
    public int next_stage;
    public int[] unlocked;
}

[Serializable]
public struct StageStar
{
    public int ID;
    public int UserID;
    public int Stage;
    public int Stars;
}

[Serializable]
public class StageStarList
{
    public StageStar[] items;
}

[Serializable]
public struct LeaderboardEntry
{
    public int rank;
    public int user_id;
    public string username;
    public int total_stars;
}

// ==========================================
// 4. คลาส APIManager หลัก
// ==========================================
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

            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================================
    // ส่วนที่ 1: ระบบล็อกอินและผู้เล่น (Auth & User)
    // ==========================================
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
        client.DefaultRequestHeaders.Remove("Authorization");
        Debug.Log("Token: [" + Token + "]");

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

    public async Task SavingGameResult(int stage, int stars)
    {
        HttpResponseMessage response = await client.PostAsync(
            "users/savinggameresult/", new StringContent(
                JsonUtility.ToJson(new { stage = stage, stars = stars }),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();
    }

    public void Logout()
    {
        Token = "";
        myData = new UserData();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        Debug.Log("ออกจากระบบ และล้างความทรงจำ HttpClient เรียบร้อย!");
    }

    // ==========================================
    // ส่วนที่ 2: ระบบจัดการโปรไฟล์ (Profile Update)
    // ==========================================
    public async Task<bool> UpdateUsername(string newUsername)
    {
        try
        {
            UpdateUsernameStruct data = new UpdateUsernameStruct { username = newUsername };

            HttpResponseMessage response = await client.PutAsync(
                "users/update-username/",
                new StringContent(
                    JsonUtility.ToJson(data),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                myData.username = newUsername;   
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

    // ==========================================
    // ส่วนที่ 3: ระบบด่าน และ ลีดเดอร์บอร์ด (Stage & Game - จากเพื่อน)
    // ==========================================
    public async Task SaveGameResult(int stars, int stage)
    {
        GameResultStruct result = new GameResultStruct
        {
            user_id = myData.id,
            stars = stars,
            stage = stage
        };

        string json = JsonUtility.ToJson(result);
        Debug.Log("Sending: " + json);

        HttpResponseMessage response = await client.PostAsync(
            "stage/save",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        );

        Debug.Log("Status: " + response.StatusCode);
        response.EnsureSuccessStatusCode();
    }

    public async Task<StageUnlockData> GetStageUnlock()
    {
        HttpResponseMessage response = await client.GetAsync("stage/unlock?user_id=" + myData.id);
        var getResponse = await response.Content.ReadAsStringAsync();
        Debug.Log(getResponse);   

        var jsonResponse = JsonUtility.FromJson<StageUnlockData>(getResponse);
        response.EnsureSuccessStatusCode();
        return jsonResponse;
    }

    public async Task<StageStar[]> GetStageStars()
    {
        HttpResponseMessage response = await client.GetAsync("stage/stars?user_id=" + myData.id);
        var json = await response.Content.ReadAsStringAsync();
        Debug.Log("Stars JSON: " + json);

        StageStar[] stars = JsonHelper.FromJson<StageStar>(json);
        return stars;
    }

    public async Task<List<LeaderboardEntry>> GetStageLeaderBoard()
    {
        HttpResponseMessage response = await client.GetAsync("stage/leaderboard");
        var json = await response.Content.ReadAsStringAsync();
        Debug.Log("Leaderboard JSON: " + json);

        LeaderboardEntry[] data = JsonHelper.FromJson<LeaderboardEntry>(json);
        return new List<LeaderboardEntry>(data);
    }

    public async Task<LeaderboardEntry> GetMyRank()
    {
        HttpResponseMessage response = await client.GetAsync("stage/leaderboard/me?user_id=" + myData.id);
        var json = await response.Content.ReadAsStringAsync();
        Debug.Log("My Rank JSON: " + json);

        LeaderboardEntry data = JsonUtility.FromJson<LeaderboardEntry>(json);
        return data;
    }

    // ==========================================
    // คลาสตัวช่วยในการแกะ JSON แบบ Array
    // ==========================================
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"items\": " + json + "}";
            return JsonUtility.FromJson<Wrapper<T>>(newJson).items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
    public async System.Threading.Tasks.Task SaveChatbotName(string newName)
    {
        if (string.IsNullOrEmpty(Token)) return;

        string url = client.BaseAddress + "users/update-chatbot"; 
        UpdateChatbotRequest req = new UpdateChatbotRequest
        {
            user_id = (uint)myData.id,
            chatbot_name = newName
        };
    string jsonData = JsonUtility.ToJson(req);

    using (UnityEngine.Networking.UnityWebRequest webRequest = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + Token);

            var operation = webRequest.SendWebRequest();
        
            while (!operation.isDone) { await System.Threading.Tasks.Task.Yield(); }

            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                myData.chatbot_name = newName; 
                PlayerPrefs.SetString("ChatbotCustomName", newName);
                PlayerPrefs.Save();
            }
        }
    }
    
}
[System.Serializable]
public class UpdateChatbotRequest
{
    public uint user_id;
    public string chatbot_name;
}