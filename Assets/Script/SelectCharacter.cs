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
    private int selectedCharacter = -1; 

    private void Start()
    {
        character1.onClick.AddListener(() => SelectCharacterOption(0));
        character2.onClick.AddListener(() => SelectCharacterOption(1));
        
        startButton.onClick.AddListener(HandleStart);
    }
    
    private void SelectCharacterOption(int character)
    {
        selectedCharacter = character;
        Debug.Log("เลือกตัวละครที่: " + character);
    }

    private async void HandleStart()
    {
        string playerName = inputName.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("ยังไม่ได้พิมพ์ชื่อเลย!");
            return; 
        }
        if (selectedCharacter == -1)
        {
            Debug.LogWarning("กรุณาเลือกตัวละครก่อน!");
            return;
        }

        Debug.Log("Starting game with character " + selectedCharacter + " and name " + playerName);
        
        try
        {
            startButton.interactable = false;

            await APIManager.Instance.ChooseCharacter(selectedCharacter, playerName);

            if (APIManager.myData.id != 0)
            {
                APIManager.myData.character = selectedCharacter;
            }
            PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
            PlayerPrefs.Save();

            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectGame");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Character selection failed: " + ex.Message);
            startButton.interactable = true; 
        }
    }
}