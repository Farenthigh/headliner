using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SettingSceneManager : MonoBehaviour
{
    [Header("Pages")]
    public GameObject profilePage;
    public GameObject changeUsernamePage;
    public GameObject changePasswordPage;
    public GameObject audioPage;
    public GameObject helpPage;
    public GameObject contactPage;

    [Header("Profile Display")]
    public TMP_Text playerNameText;
    public TMP_Text emailText;

    [Header("Change Username")]
    public TMP_InputField newUsernameInput;

    [Header("Change Password")]
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField repeatPasswordInput;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    [Header("Delete Confirm")]
    public GameObject deleteConfirmPanel;

    private void Start()
    {   
        //#if UNITY_EDITOR
        APIManager.Token = "TEST_TOKEN";
        //#endif
        // ถ้าไม่มี Token ให้กลับหน้า Login
        if (string.IsNullOrEmpty(APIManager.Token))
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }
        ShowProfile();
        LoadProfileData();
        LoadAudioSetting();
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
        changeUsernamePage.SetActive(false);
        changePasswordPage.SetActive(false);
        audioPage.SetActive(false);
        helpPage.SetActive(false);
        contactPage.SetActive(false);
    }

    public void ShowProfile()
    {
        HideAllPages();
        profilePage.SetActive(true);
    }

    public void ShowChangeUsername()
    {
        HideAllPages();
        changeUsernamePage.SetActive(true);
    }

    public void ShowChangePassword()
    {
        HideAllPages();
        changePasswordPage.SetActive(true);
    }

    public void ShowAudio()
    {
        HideAllPages();
        audioPage.SetActive(true);
    }

    public void ShowHelp()
    {
        HideAllPages();
        helpPage.SetActive(true);
    }

    public void ShowContact()
    {
        HideAllPages();
        contactPage.SetActive(true);
    }

    // =========================
    // Save Username
    // =========================
    public async void SaveUsername()
    {
        if (!string.IsNullOrEmpty(newUsernameInput.text))
        {
            await APIManager.Instance.UpdateUsername(newUsernameInput.text);
            await APIManager.Instance.GetMyData();
            LoadProfileData();
            ShowProfile();
        }
    }

    // =========================
    // Save Password
    // =========================
    public async void SavePassword()
    {
    // 🔹 เช็คว่ากรอกครบไหม
        if (string.IsNullOrEmpty(currentPasswordInput.text) ||
            string.IsNullOrEmpty(newPasswordInput.text) ||
            string.IsNullOrEmpty(repeatPasswordInput.text))
        {
            Debug.Log("Password fields are empty");
            return;
        }

    // 🔹 เช็ครหัสใหม่ตรงกันไหม
        if (newPasswordInput.text != repeatPasswordInput.text)
        {
            Debug.Log("Password not match");
            return;
        }

    // 🔹 เรียก API เปลี่ยนรหัส
        await APIManager.Instance.UpdatePassword(
            currentPasswordInput.text,
            newPasswordInput.text
        );

    // 🔹 กลับหน้า Profile
        ShowProfile();
    }
    // =========================
    // Delete Account
    // =========================
    // เปิด Popup
    public void OpenDeleteConfirm()
    {
    deleteConfirmPanel.SetActive(true);
    }

// กดยกเลิก
    public void CancelDelete()
    {
        deleteConfirmPanel.SetActive(false);
    }

// กดยืนยันลบ
    public async void ConfirmDelete()
    {
        deleteConfirmPanel.SetActive(false);

        await APIManager.Instance.DeleteAccount();
        SceneManager.LoadScene("LoginScene");
    }

    // =========================
    // Logout
    // =========================
    public void Logout()
    {
        APIManager.Token = null;
        SceneManager.LoadScene("LoginScene");
    }

    // =========================
    // ปุ่ม close
    // =========================
    public void PreviousPage()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene");
        SceneManager.LoadScene(previousScene);
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
}