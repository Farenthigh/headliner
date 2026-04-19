using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class SettingSceneManager : MonoBehaviour
{
    private bool openedProfile;
    private bool openedAudio;
    private bool openedHelp;
    private bool openedContact;
    [Header("Pages")]
    public GameObject profilePage;
    public GameObject editProfilePage;
    public GameObject audioPage;
    public GameObject helpPage;
    public GameObject contactPage;

    [Header("Sidebar Highlight")]
    public GameObject profileHighlight;
    public GameObject audioHighlight;
    public GameObject contactHighlight;
    public GameObject helpHighlight;

    [Header("Profile Display")]
    public TMP_Text playerNameText;
    public TMP_Text emailText;

    [Header("Edit Profile")]
    public TMP_InputField newUsernameInput;
    public TMP_InputField emailInput; 
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField repeatPasswordInput;
   

    public TMP_Text errorText;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    [Header("Delete Confirm")]
    public GameObject deleteConfirmPanel;

    [Header("Logout Confirm")]
    public GameObject logoutConfirmPanel;

    [Header("Success Popup")]
    public GameObject usernameSuccessPanel;

    [Header("Contact Form")]
    public TMP_InputField subjectInput;
    public TMP_InputField descriptionInput;
    public GameObject panelSuccessContact;
    public GameObject panelFailedContact;
    public float contactPopupTime = 5f;
    [System.Serializable]
    public class ContactData
    {
        public string Subject;
        public string Description;
    }
    private void Start()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(APIManager.Token))
        {
            APIManager.Token = "TEST_TOKEN";
            APIManager.myData = new UserData
            {
                username = "TestPlayer",
                email = "test@email.com",
                character = 1
            };
            Debug.LogWarning("ใช้ข้อมูลจำลองเพราะไม่มี Token จริงส่งมา");
        }
#endif

        if (string.IsNullOrEmpty(APIManager.Token))
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        ShowProfile();
        LoadProfileData();
        LoadAudioSetting();

        openedProfile = PlayerPrefs.GetInt("Open_Profile", 0) == 1;
        openedAudio = PlayerPrefs.GetInt("Open_Audio", 0) == 1;
        openedHelp = PlayerPrefs.GetInt("Open_Help", 0) == 1;
        openedContact = PlayerPrefs.GetInt("Open_Contact", 0) == 1;
    }

    // =========================
    // Highlight Control
    // =========================

    void ResetHighlight()
    {
        profileHighlight.SetActive(false);
        audioHighlight.SetActive(false);
        contactHighlight.SetActive(false);
        helpHighlight.SetActive(false);
    }

    // =========================
    // โหลดข้อมูลโปรไฟล์
    // =========================

    void LoadProfileData()
    {
        if (!string.IsNullOrEmpty(APIManager.myData.username))
            playerNameText.text = APIManager.myData.username;
        else
            playerNameText.text = "Player Name";

        if (!string.IsNullOrEmpty(APIManager.myData.email))
            emailText.text = APIManager.myData.email;
        else
            emailText.text = "E-mail";
    }

    // =========================
    // เปลี่ยนหน้า
    // =========================

    void HideAllPages()
    {
        profilePage.SetActive(false);
        editProfilePage.SetActive(false);
        audioPage.SetActive(false);
        helpPage.SetActive(false);
        contactPage.SetActive(false);
    }

    public void ShowProfile()
    {
        HideAllPages();
        ResetHighlight();

        profilePage.SetActive(true);
        profileHighlight.SetActive(true);

        openedProfile = true;
        PlayerPrefs.SetInt("Open_Profile", 1);
        CheckFullExplorer();
    }

    public void ShowEditProfile()
    {
        HideAllPages();
        ResetHighlight();

        editProfilePage.SetActive(true);
        profileHighlight.SetActive(true);
        
        errorText.text = "";
        newUsernameInput.text = APIManager.myData.username;
        emailInput.text = APIManager.myData.email;

        currentPasswordInput.text = "";
        newPasswordInput.text = "";
        repeatPasswordInput.text = "";
        
    }

    public void ShowAudio()
    {
        HideAllPages();
        ResetHighlight();

        audioPage.SetActive(true);
        audioHighlight.SetActive(true);

        openedAudio = true;
        PlayerPrefs.SetInt("Open_Audio", 1);
        PlayerPrefs.Save();
        CheckFullExplorer();
    }

    public void ShowHelp()
    {
        HideAllPages();
        ResetHighlight();

        helpPage.SetActive(true);
        helpHighlight.SetActive(true);

        openedHelp = true;
        PlayerPrefs.SetInt("Open_Help", 1);
        PlayerPrefs.Save();
        CheckFullExplorer();    
    }

    public void ShowContact()
    {
        HideAllPages();
        ResetHighlight();

        contactPage.SetActive(true);
        contactHighlight.SetActive(true);

        openedContact = true;
        PlayerPrefs.SetInt("Open_Contact", 1);
        PlayerPrefs.Save();
        CheckFullExplorer();
    }

    // =========================
    public async void SaveChanges()
{   
    Debug.Log("SAVE PRESSED");
    errorText.text = "";

    bool usernameChanged = newUsernameInput.text.Trim() != APIManager.myData.username;
    bool passwordChanged = !string.IsNullOrEmpty(newPasswordInput.text);

    if (!usernameChanged && !passwordChanged)
    {
        errorText.text = "No changes made";
        return;
    }

    if (usernameChanged)
    {
        string username = newUsernameInput.text.Trim();

        bool success = await APIManager.Instance.UpdateUsername(username);

        if (!success)
        {
            errorText.text = "Username already taken";
            return;
        }
    }

    if (passwordChanged)
    {
        if (string.IsNullOrEmpty(currentPasswordInput.text))
        {
            errorText.text = "Enter current password";
            return;
        }

        if (newPasswordInput.text != repeatPasswordInput.text)
        {
            errorText.text = "Passwords do not match";
            return;
        }

        bool success = await APIManager.Instance.UpdatePassword(
            currentPasswordInput.text,
            newPasswordInput.text
        );

        if (!success)
        {
            errorText.text = "Incorrect current password";
            return;
        }
    }

    await APIManager.Instance.GetMyData();
    LoadProfileData();

    currentPasswordInput.text = "";
    newPasswordInput.text = "";
    repeatPasswordInput.text = "";

    ShowProfile();
    StartCoroutine(ShowSuccessPopup());
}
public void CancelEditProfile()
{
    ShowProfile();
}

IEnumerator ShowSuccessPopup()
{
    usernameSuccessPanel.SetActive(true);

    yield return new WaitForSeconds(3f);

    usernameSuccessPanel.SetActive(false);
}



     // =========================
    // Delete Account
    // =========================

    public void OpenDeleteConfirm()
    {
        deleteConfirmPanel.SetActive(true);
    }

    public void CancelDelete()
    {
        deleteConfirmPanel.SetActive(false);
    }

    public async void ConfirmDelete()
    {
        deleteConfirmPanel.SetActive(false);

        await APIManager.Instance.DeleteAccount();
        if (APIManager.Instance != null)
        {
            APIManager.Instance.Logout(); 
        }

        SceneManager.LoadScene("LoginScene");
    }

    // =========================
    // Logout
    // =========================

    public void OpenLogoutConfirm()
    {
        logoutConfirmPanel.SetActive(true);
    }

    public void CancelLogout()
    {
        logoutConfirmPanel.SetActive(false);
    }

    public void ConfirmLogout()
    {
        logoutConfirmPanel.SetActive(false);
        Logout();
    }

    public void Logout()
    {
        if (APIManager.Instance != null)
        {
            APIManager.Instance.Logout();
        }
        else
        {
            APIManager.Token = null; 
        }

        SceneManager.LoadScene("LoginScene");
    }

    // =========================
    // Back Button
    // =========================

    public void PreviousPage()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");
        if (string.IsNullOrEmpty(previousScene) || previousScene == "-1")
        {
            SceneManager.LoadScene(0); 
            return;
        }

        if (int.TryParse(previousScene, out int sceneIndex))
        {
            SceneManager.LoadScene(Mathf.Max(0, sceneIndex));
        }
        else
        {
            SceneManager.LoadScene(previousScene);
        }
    }

    // =========================
    // AUDIO
    // =========================

    void LoadAudioSetting()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
        masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);

        musicSlider.onValueChanged.AddListener(delegate { SaveAudio(); });
        sfxSlider.onValueChanged.AddListener(delegate { SaveAudio(); });
        masterSlider.onValueChanged.AddListener(delegate { SaveAudio(); });
    }

    void SaveAudio()
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
        PlayerPrefs.SetFloat("Master", masterSlider.value);
    }
    public void SubmitContact()
    {
        string subject = subjectInput.text.Trim();
        string description = descriptionInput.text.Trim();

        if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
        {
            ShowContactPanel(panelFailedContact);
            return;
        }

        StartCoroutine(SendContactToAPI(subject, description));
    }
    void ShowContactPanel(GameObject panel)
    {
    // ปิดทั้งสองก่อน (กันซ้อน)
        panelSuccessContact.SetActive(false);
        panelFailedContact.SetActive(false);

        panel.SetActive(true);
        StartCoroutine(HideContactPanel(panel));
    }
    IEnumerator HideContactPanel(GameObject panel)
    {
        yield return new WaitForSeconds(contactPopupTime);
        panel.SetActive(false);
    }
    IEnumerator SendContactToAPI(string subject, string description)
    {
        string url = "http://localhost:8080/contact"; // 🔥 เปลี่ยนตาม backend จริง

    // JSON ที่จะส่ง (ต้องตรงกับ backend)
        string json = JsonUtility.ToJson(new ContactData
        {
            Subject = subject,
            Description = description
        });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowContactPanel(panelSuccessContact);
        }
        else
        {
            ShowContactPanel(panelFailedContact);
        }
    }
}

