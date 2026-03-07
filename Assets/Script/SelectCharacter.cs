using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class SelectCharacter : MonoBehaviour
{
    [SerializeField] private Button character1;
    [SerializeField] private Button character2;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button startButton;
    private int selectedCharacter = 0;

    private void Start()
    {
        character1.onClick.AddListener(() => SelectCharacterOption(1));
        character2.onClick.AddListener(() => SelectCharacterOption(2));
        startButton.onClick.AddListener(HandleStart);
    }
    private void SelectCharacterOption(int character)
    {
        selectedCharacter = character;
    }

    private async void HandleStart()
    {
        string playerName = inputName.text;
        Debug.Log("Starting game with character " + selectedCharacter + " and name " + playerName);
        try
        {
            await APIManager.Instance.ChooseCharacter(selectedCharacter, playerName);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectGame");
        }

        catch (System.Exception ex)
        {
            Debug.Log("Character selection failed: " + ex.Message);
        }
    }


}
