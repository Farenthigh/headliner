using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void HandleStart()
    {
        string playerName = inputName.text;
        Debug.Log($"Starting game with Character: {selectedCharacter} and Name: {playerName}");
    }


}
