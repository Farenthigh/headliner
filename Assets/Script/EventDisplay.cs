using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EventDisplay : MonoBehaviour
{
    public static EventDisplay Instance;

    [Header("UI Objects")]
    public GameObject panel;
    public GameObject cardBack;
    public GameObject cardFront;

    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private bool isWaitingForInput = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowEvent(RandomEvent eventData)
    {
        Debug.Log("Showing Event: " + eventData.eventName);
        // Set Front card details but hide it
        // nameText.text = eventData.eventName;
        // descriptionText.text = eventData.description;
        // iconImage.sprite = eventData.icon;

        // Show panel and animate card flip start with back to front
        panel.SetActive(true);
        cardBack.SetActive(true);
        cardFront.SetActive(false);

        StartCoroutine(RevealProcess());
    }

    IEnumerator RevealProcess()
    {
        // // Wait for 1 second before flipping
        // yield return new WaitForSeconds(1f);

        // wait for player click to flip the card
        isWaitingForInput = true;

        // Add a listener to the cardBack button to set isWaitingForInput to false when clicked
        Button cardBackButton = cardBack.GetComponent<Button>();
        void OnCardBackClicked()
        {
            isWaitingForInput = false;
        }
        cardBackButton.onClick.AddListener(OnCardBackClicked);

        while (isWaitingForInput)
        {
            yield return null;
        }

        // Remove the listener after input is received
        cardBackButton.onClick.RemoveListener(OnCardBackClicked);

        // Flip the card to reveal front
        RevealCard();
    }

    void RevealCard()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
        Button cardFrontButton = cardFront.GetComponent<Button>();
        cardFrontButton.onClick.AddListener(OnCardFrontClicked);

        //Sound effect can be added here like "ฟึ่บ"
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
    public void OnCardFrontClicked()
    {
        // Close the panel and trigger the event effect
        Debug.Log("Card front clicked, closing panel and triggering event effect.");
        ClosePanel();
        SavingGameLogicManager.Instance.SetIsPaused(false);
    }
}