using System;
using System.Net.Http;
using System.Collections.Generic;
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
        response.EnsureSuccessStatusCode();
        return response.Headers.Location;
    }

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
        "/stage/save",
        new StringContent(json, System.Text.Encoding.UTF8, "application/json")
    );

    Debug.Log("Status: " + response.StatusCode);

    response.EnsureSuccessStatusCode();
}

public async Task<StageUnlockData> GetStageUnlock()
{
    HttpResponseMessage response = await client.GetAsync(
        "stage/unlock?user_id=" + myData.id
    );

    var getResponse = await response.Content.ReadAsStringAsync();

    Debug.Log(getResponse);   // ดู JSON ที่ backend ส่งมา

    var jsonResponse = JsonUtility.FromJson<StageUnlockData>(getResponse);

    response.EnsureSuccessStatusCode();

    return jsonResponse;
}

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

public async Task<StageStar[]> GetStageStars()
{
    HttpResponseMessage response = await client.GetAsync(
        "stage/stars?user_id=" + myData.id
    );

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
        HttpResponseMessage response = await client.GetAsync(
            "stage/leaderboard/me?user_id=" + myData.id
        );

        var json = await response.Content.ReadAsStringAsync();
        Debug.Log("My Rank JSON: " + json);

        LeaderboardEntry data = JsonUtility.FromJson<LeaderboardEntry>(json);

        return data;
    }
}
