using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
[Serializable]
public struct SavingGameResultStruct
{
    public int user_id;
    public int stage;
    public int stars;
}


public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
    public static string Token;
    public static UserData myData;
    static HttpClient client = new HttpClient();

    private void Awake()
    {
        client.BaseAddress = new Uri("http://localhost:8080/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        response.EnsureSuccessStatusCode();
        return response.Headers.Location;
    }

    public async Task SavingGameResult(int stage, int stars)
    {
        SavingGameResultStruct result = new SavingGameResultStruct
        {
            user_id = myData.id,
            stage = stage,
            stars = stars
        };
        HttpResponseMessage response = await client.PostAsync(
            "saving-stage/save", new StringContent(
                JsonUtility.ToJson(result),
                System.Text.Encoding.UTF8,
                "application/json"));
        response.EnsureSuccessStatusCode();
    }
    [Serializable]
    public struct GetSavingGameStarStruct
    {
        public int id;      // Matches "ID" or "id" from JSON
        public int user_id; // Matches "user_id"
        public int stage;   // Matches "stage"
        public int stars;   // Matches "stars"
    }

    [Serializable]
    public class StarDataWrapper
    {
        public List<GetSavingGameStarStruct> items;
    }
    public async Task<int> GetSavingGameStar(int stage)
    {
        try
        {
            // HttpResponseMessage response = await client.GetAsync($"saving-stage/stars?user_id={myData.id}");
            HttpResponseMessage response = await client.GetAsync("saving-stage/stars?user_id=99");

            if (!response.IsSuccessStatusCode) return -1;

            var getResponse = await response.Content.ReadAsStringAsync();

            // Wrap the raw array string for JsonUtility
            string wrappedJson = "{ \"items\": " + getResponse + " }";
            var jsonResponse = JsonUtility.FromJson<StarDataWrapper>(wrappedJson);

            // Check if list is null or empty
            if (jsonResponse?.items == null || jsonResponse.items.Count == 0)
            {
                return -1;
            }

            // Find the specific stage
            // Note: Using lowercase .stage to match the new struct
            int index = jsonResponse.items.FindIndex(s => s.stage == stage);

            if (index == -1)
            {
                return -1; // Stage not found in the list
            }

            return jsonResponse.items[index].stars;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching star data: {e.Message}");
            return -1;
        }
    }
    [Serializable]
    public class StageResponse
    {
        public int next_stage;
        public int now_stage;
        public int[] unlocked; // Or List<int>
    }
    public async Task<int> GetlatestStage()
    {
        // 1. Fetch the data
        // HttpResponseMessage response = await client.GetAsync("saving-stage/unlock?user_id={myData.id}"); real
        HttpResponseMessage response = await client.GetAsync("saving-stage/unlock?user_id=99");

        response.EnsureSuccessStatusCode();

        // 2. Read the raw string
        string getResponse = await response.Content.ReadAsStringAsync();

        // 3. Deserialize into our custom class
        // Note: If your API wraps this in a "data" field, use ApiResponse<StageResponse>
        // Based on the JSON you provided, it looks like a direct object:
        StageResponse jsonResponse = JsonUtility.FromJson<StageResponse>(getResponse);

        // 4. Return just the next_stage
        return jsonResponse.next_stage;
    }
}
