using TMPro;
using UnityEngine;

public class ChangeUsernameUI : MonoBehaviour
{
    public TMP_InputField newUsernameInput;
    public GameObject changeUsernamePage;
    public GameObject profilePage;
    public GameObject successPopup;

    public async void SaveUsername()
    {
        string newName = newUsernameInput.text;

        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("Username is empty");
            return;
        }

        if (APIManager.Instance == null)
        {
            Debug.LogError("APIManager not found");
            return;
        }

    #if UNITY_EDITOR
        APIManager.myData.username = newName;
    #else
        await APIManager.Instance.ChooseCharacter(
            APIManager.myData.character,
            newName
        );

        await APIManager.Instance.GetMyData();
    #endif


        successPopup.SetActive(true);
    }
    public void GoToProfile()
    {
        successPopup.SetActive(false);
        changeUsernamePage.SetActive(false);
        profilePage.SetActive(true);
    }
}