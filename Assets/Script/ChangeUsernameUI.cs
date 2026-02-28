using TMPro;
using UnityEngine;

public class ChangeUsernameUI : MonoBehaviour
{
    public TMP_InputField newUsernameInput;
    public GameObject changeUsernamePage;
    public GameObject profilePage;

    public async void SaveUsername()
    {
        string newName = newUsernameInput.text;

        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("Username is empty");
            return;
        }

        await APIManager.Instance.ChooseCharacter(
            APIManager.myData.character,
            newName
        );

        await APIManager.Instance.GetMyData();

        changeUsernamePage.SetActive(false);
        profilePage.SetActive(true);
    }
}