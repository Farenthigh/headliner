using UnityEngine;

public class ProfilePageController : MonoBehaviour
{
    public GameObject profilePage;

    public async void OpenProfile()
    {
        await APIManager.Instance.GetMyData();
        profilePage.SetActive(true);
    }
}