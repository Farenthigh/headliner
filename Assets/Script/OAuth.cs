using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.Networking;
using System.Collections;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

[System.Serializable]
public class GoogleTokenResponse
{
    public string access_token;
    public string id_token;
    public int expires_in;
    public string token_type;
    public string refresh_token;
}
[System.Serializable]
public class GoogleSecrets
{
    public string clientId;
    public string clientSecret;
}

public class OAuth : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    private string clientId = "";
    private string clientSecret = "";
    private string redirectUri = "http://localhost:54321/";
    [SerializeField] private AuthMode currentAuthMode;

    [Serializable]
    public enum AuthMode { Login, Signup }

    private void Start()
    {
        TextAsset secretFile = Resources.Load<TextAsset>("google-secret.json");
        if (secretFile != null)
        {
            // Parse your JSON here
            var secrets = JsonUtility.FromJson<GoogleSecrets>(secretFile.text);
            clientId = secrets.clientId;
            clientSecret = secrets.clientSecret;
            Debug.Log("Client ID : " + clientId);
            Debug.Log("Client Secret : " + clientSecret);
        }
        loginButton.onClick.AddListener(() => StartAuthFlow(currentAuthMode));
        // Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
    }

    public async void StartAuthFlow(AuthMode mode)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);
        try
        {
            listener.Start();

            string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile";
            Application.OpenURL(authUrl);

            // รอรับข้อมูล
            var context = await listener.GetContextAsync();
            string code = context.Request.QueryString.Get("code");

            // ส่ง Response กลับไปที่ Browser ก่อน
            var response = context.Response;
            string responseString = "<html><body><h1>Login Success!</h1><p>You can close this tab.</p></body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close(); // ปิด Stream ของข้อมูล

            // หยุด Listener หลังจากส่งข้อมูลเสร็จแล้ว
            listener.Stop();
            listener.Close();

            if (!string.IsNullOrEmpty(code))
            {
                StartCoroutine(ExchangeCodeForFirebase(code));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Listener Error: " + e.Message);
            if (listener.IsListening) listener.Stop();
        }
    }
    IEnumerator ExchangeCodeForFirebase(string code)
    {
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("grant_type", "authorization_code");

        using (UnityWebRequest www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // ใช้ JsonUtility แทน Newtonsoft
                string jsonResponse = www.downloadHandler.text;
                GoogleTokenResponse tokens = JsonUtility.FromJson<GoogleTokenResponse>(jsonResponse);

                string idToken = tokens.id_token;

                if (!string.IsNullOrEmpty(idToken))
                {
                    SignInToFirebase(idToken);
                }
                else
                {
                    Debug.LogError("สกัด id_token ออกมาไม่ได้ (JSON อาจจะผิดรูปแบบ)");
                }
            }
            else
            {
                Debug.LogError("Error: " + www.downloadHandler.text);
            }
        }
    }
    void SignInToFirebase(string idToken)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        // 1. สร้าง Credential จาก Google idToken
        Firebase.Auth.Credential credential =
            Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);

        // 2. ต้องสั่ง SignIn ด้วย Credential ใหม่ก่อนเสมอ!!
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignIn was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignIn encountered an error: " + task.Exception);
                return;
            }

            // 3. เมื่อล็อกอินสำเร็จ เราจะได้ AuthResult ซึ่งมี User ใหม่อยู่ข้างใน
            Firebase.Auth.AuthResult result = task.Result;
            Firebase.Auth.FirebaseUser newUser = result.User;

            Debug.LogFormat("Firebase User signed in: {0} ({1})", newUser.DisplayName, newUser.Email);

            // 4. ดึง Token จาก newUser (ตัวใหม่ล่าสุด) และสั่ง Force Refresh (true)
            newUser.TokenAsync(true).ContinueWithOnMainThread(tokenTask =>
            {
                if (tokenTask.IsCompleted && !tokenTask.IsFaulted)
                {
                    string freshToken = tokenTask.Result;
                    Debug.Log("Fresh Token obtained for: " + newUser.Email);

                    // 5. ส่ง Token ใหม่ไปที่ Go Backend
                    StartCoroutine(SendTokenToBackend(freshToken));
                }
            });
        });
    }
    IEnumerator SendTokenToBackend(string idToken)
    {
        Debug.Log("Sending idToken to backend: " + idToken);

        Task<Uri> apiTask;

        // เลือก API ตาม Mode
        if (currentAuthMode == AuthMode.Signup)
        {
            apiTask = APIManager.Instance.RegisterWithGmail(idToken);
        }
        else
        {
            apiTask = APIManager.Instance.LoginWithGmail(idToken);
        }

        // รอจนกว่า Task จะทำงานเสร็จโดยไม่ทำให้เกมค้าง
        while (!apiTask.IsCompleted)
        {
            yield return null;
        }

        if (apiTask.IsFaulted)
        {
            Debug.LogError("Login Error: " + apiTask.Exception.Flatten().InnerException.Message);
        }
        else
        {
            // ดึงข้อมูลตัวละครต่อ


            // เช็คเงื่อนไขเปลี่ยนฉาก
            if (APIManager.myData.character == 0 && string.IsNullOrEmpty(APIManager.myData.username))
            {
                if (currentAuthMode == AuthMode.Signup)
                    UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
                else if (currentAuthMode == AuthMode.Login)
                {
                    var dataTask = APIManager.Instance.GetMyData();
                    while (!dataTask.IsCompleted) yield return null;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCharacterScene");
                }
            }
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
        }
    }
}