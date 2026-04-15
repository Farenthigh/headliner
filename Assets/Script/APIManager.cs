using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Data.Common;

// ==========================================
// 1. ALL DATA STRUCTURES (COMBINED)
// ==========================================

[Serializable]
public struct ApiResponse<T>
{
    public T data;
    public string error;
    public string message;
}

[Serializable]
public struct TokenData { public string token; }

[Serializable]
public struct UserData
{
    public int id;
    public string email;
    public string username;
    public int character;
    public string chatbot_name;
}

public struct RegisterStruct { public string email; public string password; public string confirm_password; }
public struct LoginStruct { public string email; public string password; }
public struct CharacterStruct { public string username; public int character; }

[Serializable]
public struct UpdateUsernameStruct { public string username; }

[Serializable]
public struct UpdatePasswordStruct { public string currentPassword; public string newPassword; }

[Serializable]
public struct GameResultStruct
{
    public int user_id;
    public int stars;
    public int stage;
}

[Serializable]
public struct SavingGameResultStruct
{
    public int user_id;
    public int stage;
    public int stars;
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
public class StageStarList { public StageStar[] items; }

[Serializable]
public struct LeaderboardEntry
{
    public int rank;
    public int user_id;
    public string username;
    public int total_stars;
}

[Serializable]
public struct GetSavingGameStarStruct
{
    public int id;
    public int user_id;
    public int stage;
    public int stars;
}

[Serializable]
public class StarDataWrapper { public List<GetSavingGameStarStruct> items; }

[Serializable]
public class StageResponse
{
    public int next_stage;
    public int now_stage;
    public int[] unlocked;
}

[Serializable]
public class UpdateChatbotRequest
{
    public uint user_id;
    public string chatbot_name;
}

// ==========================================
// 2. APIManager (ALL FUNCTIONS MERGED)
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
        // Initial setup from both versions
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

    // --- Version A & B: Auth & Core ---

    public async Task<Uri> Register(RegisterStruct register)
    {
        HttpResponseMessage response = await client.PostAsync(
            "users/register/", new StringContent(
                JsonUtility.ToJson(register), System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return response.Headers.Location;
    }

    public async Task<Uri> Login(string email, string password)
    {
        HttpResponseMessage response = await client.PostAsync(
            "users/login/", new StringContent(
                JsonUtility.ToJson(new LoginStruct { email = email, password = password }),
                System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        var postResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<ApiResponse<TokenData>>(postResponse);
        Token = jsonResponse.data.token;

        // Shared header update logic
        string cleanToken = Token.Trim().Replace("\"", "");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cleanToken);
        return response.Headers.Location;
    }

    public async Task<Uri> ChooseCharacter(int character, string name)
    {
        Debug.Log("Choosing character " + character + " with name " + name);
        HttpResponseMessage response = await client.PostAsync(
            "users/createcharacter/", new StringContent(
                JsonUtility.ToJson(new CharacterStruct { username = name, character = character }),
                System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
        return response.Headers.Location;
    }

    public async Task<Uri> GetMyData()
    {
        client.DefaultRequestHeaders.Remove("Authorization");
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
        Token = "";
        myData = new UserData();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        Debug.Log("ออกจากระบบ เรียบร้อย!");
    }

    // --- Profile Management Section ---

    public async Task<bool> UpdateUsername(string newUsername)
    {
        try
        {
            UpdateUsernameStruct data = new UpdateUsernameStruct { username = newUsername };
            HttpResponseMessage response = await client.PutAsync("users/update-username/",
                new StringContent(JsonUtility.ToJson(data), System.Text.Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode) { myData.username = newUsername; }
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Debug.Log("Update username error: " + e.Message); return false; }
    }

    public async Task<bool> UpdatePassword(string currentPassword, string newPassword)
    {
        try
        {
            UpdatePasswordStruct data = new UpdatePasswordStruct { currentPassword = currentPassword, newPassword = newPassword };
            HttpResponseMessage response = await client.PutAsync("users/update-password/",
                new StringContent(JsonUtility.ToJson(data), System.Text.Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Debug.Log("Update password error: " + e.Message); return false; }
    }

    public async Task<bool> DeleteAccount()
    {
        try
        {
            HttpResponseMessage response = await client.DeleteAsync("users/delete/");
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) { Debug.Log("Delete account error: " + e.Message); return false; }
    }

    public async Task SaveChatbotName(string newName)
    {
        if (string.IsNullOrEmpty(Token)) return;
        string url = client.BaseAddress + "users/update-chatbot";
        UpdateChatbotRequest req = new UpdateChatbotRequest { user_id = (uint)myData.id, chatbot_name = newName };
        string jsonData = JsonUtility.ToJson(req);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + Token);

            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) { await Task.Yield(); }
            if (webRequest.result != UnityWebRequest.Result.Success) { Debug.LogError(webRequest.error); }
            else
            {
                myData.chatbot_name = newName;
                PlayerPrefs.SetString("ChatbotCustomName", newName);
                PlayerPrefs.Save();
            }
        }
    }

    // Version B variant (using SavingGameResultStruct)
    public async Task SavingGameResult(int stage, int stars)
    {
        SavingGameResultStruct result = new SavingGameResultStruct { user_id = myData.id, stage = stage, stars = stars };
        HttpResponseMessage response = await client.PostAsync(
            "/saving-stage/save", new StringContent(
                JsonUtility.ToJson(result), System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    // Version from "Friend" logic
    public async Task SaveGameResult(int stars, int stage)
    {
        GameResultStruct result = new GameResultStruct { user_id = myData.id, stars = stars, stage = stage };
        HttpResponseMessage response = await client.PostAsync(
            "/stage/save", new StringContent(JsonUtility.ToJson(result), System.Text.Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();
    }

    // --- Stage & Star Fetching ---

    public async Task<int> GetSavingGameStar(int stage)
    {
        try
        {
            // Using ID 99 as per requested snippet redundancy
            HttpResponseMessage response = await client.GetAsync("saving-stage/stars?user_id=" + myData.id);
            if (!response.IsSuccessStatusCode) return -1;
            var getResponse = await response.Content.ReadAsStringAsync();
            string wrappedJson = "{ \"items\": " + getResponse + " }";
            var jsonResponse = JsonUtility.FromJson<StarDataWrapper>(wrappedJson);
            if (jsonResponse?.items == null || jsonResponse.items.Count == 0) return -1;
            int index = jsonResponse.items.FindIndex(s => s.stage == stage);
            return (index == -1) ? -1 : jsonResponse.items[index].stars;
        }
        catch (Exception e) { Debug.LogError($"Error fetching star data: {e.Message}"); return -1; }
    }

    public async Task<int> GetlatestStage()
    {
        HttpResponseMessage response = await client.GetAsync("saving-stage/unlock?user_id=" + myData.id);
        response.EnsureSuccessStatusCode();
        string getResponse = await response.Content.ReadAsStringAsync();
        StageResponse jsonResponse = JsonUtility.FromJson<StageResponse>(getResponse);
        return jsonResponse.now_stage;
    }

    public async Task<StageUnlockData> GetStageUnlock()
    {
        HttpResponseMessage response = await client.GetAsync("stage/unlock?user_id=" + myData.id);
        var getResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<StageUnlockData>(getResponse);
        response.EnsureSuccessStatusCode();
        return jsonResponse;
    }

    public async Task<StageStar[]> GetStageStars()
    {
        HttpResponseMessage response = await client.GetAsync("stage/stars?user_id=" + myData.id);
        var json = await response.Content.ReadAsStringAsync();
        StageStar[] stars = JsonHelper.FromJson<StageStar>(json);
        return stars;
    }

    // --- Leaderboard Section ---

    public async Task<List<LeaderboardEntry>> GetStageLeaderBoard()
    {
        HttpResponseMessage response = await client.GetAsync("stage/leaderboard");
        var json = await response.Content.ReadAsStringAsync();
        LeaderboardEntry[] data = JsonHelper.FromJson<LeaderboardEntry>(json);
        return new List<LeaderboardEntry>(data);
    }

    public async Task<LeaderboardEntry> GetMyRank()
    {
        HttpResponseMessage response = await client.GetAsync("stage/leaderboard/me?user_id=" + myData.id);
        var json = await response.Content.ReadAsStringAsync();
        return JsonUtility.FromJson<LeaderboardEntry>(json);
    }

    public async Task<Uri> LoginWithGmail(string firebaseIdToken)
    {
        client.DefaultRequestHeaders.Authorization = null;
        // 1. ตั้งค่า Header สำหรับส่ง Firebase Token (ID Token)
        // Middleware ใน Go ของเราจะรอรับ Bearer <token> จากตรงนี้
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firebaseIdToken);

        // 2. ยิง Request ไปที่ Endpoint ใหม่
        // เนื่องจากเราส่ง Token ผ่าน Header แล้ว Body อาจจะส่งเป็น JSON เปล่าๆ {} 
        // หรือถ้า API ฝั่ง Go ไม่ได้ใช้ Body ก็ส่ง StringContent เปล่าไปได้ครับ
        HttpResponseMessage response = await client.PostAsync(
            "users/login-with-google/", new StringContent(
                "{}", // ส่ง JSON เปล่าๆ ไปถ้าฝั่ง Go ไม่ได้รับค่าจาก Body
                System.Text.Encoding.UTF8,
                "application/json"));

        // 3. ตรวจสอบ Status Code
        response.EnsureSuccessStatusCode();

        // 4. อ่าน Response เพื่อเอา System JWT Token มาเก็บไว้ใช้ต่อ
        var postResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<ApiResponse<TokenData>>(postResponse);

        // 5. อัปเดต Token ของระบบเราเอง (ที่ได้จาก Go) ลงใน Client 
        // เพื่อให้ Request ครั้งต่อๆ ไปใช้ Token ของระบบเราแทน Firebase Token
        Token = jsonResponse.data.token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        return response.Headers.Location;
    }
    public async Task<Uri> RegisterWithGmail(string firebaseIdToken)
    {
        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firebaseIdToken);

        HttpResponseMessage response = await client.PostAsync(
            "users/register-with-google/", new StringContent(
                "{}", // ส่ง JSON เปล่าๆ ไปถ้าฝั่ง Go ไม่ได้รับค่าจาก Body
                System.Text.Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();

        var postResponse = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonUtility.FromJson<ApiResponse<TokenData>>(postResponse);

        Token = jsonResponse.data.token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        return response.Headers.Location;
    }

    // --- Static Helper ---

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"items\": " + json + "}";
            return JsonUtility.FromJson<Wrapper<T>>(newJson).items;
        }

        [Serializable]
        private class Wrapper<T> { public T[] items; }
    }
}