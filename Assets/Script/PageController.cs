using UnityEngine;

public class PageController : MonoBehaviour
{
    public GameObject profilePage;
    public GameObject changeUsernamePage;
    public GameObject changePasswordPage;

    public void OpenChangeUsername()
    {
        profilePage.SetActive(false);
        changeUsernamePage.SetActive(true);
    }

    public void OpenChangePassword()
    {
        profilePage.SetActive(false);
        changePasswordPage.SetActive(true);
    }

    public void BackToProfile()
    {
        changeUsernamePage.SetActive(false);
        changePasswordPage.SetActive(false);
        profilePage.SetActive(true);
    }
}