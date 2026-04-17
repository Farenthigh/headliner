using System.Collections;
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
    [SerializeField] private GameObject triangle1;
    [SerializeField] private GameObject triangle2;
    private int selectedCharacter = 0;

    private void Start()
    {
        character1.onClick.AddListener(() => SelectCharacterOption(1));
        character2.onClick.AddListener(() => SelectCharacterOption(2));
        startButton.onClick.AddListener(HandleStart);

        // ซ่อนทั้งหมดก่อน
        triangle1.SetActive(false);
        triangle2.SetActive(false);
    }

    IEnumerator PopEffect(Transform target)
    {
        target.localScale = Vector3.one * 0.8f;

        float time = 0f;
        float duration = 0.15f;

        while (time < duration)
        {
            target.localScale = Vector3.Lerp(
                target.localScale,
                Vector3.one * 1.2f,
                time / duration
            );
            time += Time.deltaTime;
            yield return null;
        }

        target.localScale = Vector3.one * 1.2f;
    }

    private void SelectCharacterOption(int character)
    {
        selectedCharacter = character;

        if (character == 1)
        {
            triangle1.SetActive(true);
            triangle2.SetActive(false);

            // ขยายสามเหลี่ยมนิดนึง
            StartCoroutine(PopEffect(triangle1.transform)); // 👈 ใส่ตรงนี้
        }

        else
        {
            triangle2.SetActive(true);
            triangle1.SetActive(false);

            StartCoroutine(PopEffect(triangle2.transform)); // 👈 ใส่ตรงนี้
        }
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
