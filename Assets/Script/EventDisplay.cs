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

    [Header("Rarity Front Sprites")]
    public Sprite commonFrame;
    public Sprite uncommonFrame;
    public Sprite rareFrame;
    public Image cardFrame;

    [Header("Rarity Back Sprites")]
    public Sprite commonBack;
    public Sprite uncommonBack;
    public Sprite rareBack;
    public Image cardBackImage;

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
    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowEvent(Event eventData)
    {
        Debug.Log("Showing Event: " + eventData.eventName);
        // Set Front card details but hide it
        // nameText.text = eventData.eventName;
        // descriptionText.text = eventData.description;
        // iconImage.sprite = eventData.icon;

        if (cardFrame != null)
        {
            if (eventData.rarity == EventRarity.Common)
            {
                if (cardFrame != null) cardFrame.sprite = commonFrame;
                if (cardBackImage != null) cardBackImage.sprite = commonBack;
            }
            else if (eventData.rarity == EventRarity.Uncommon)
            {
                if (cardFrame != null) cardFrame.sprite = uncommonFrame;
                if (cardBackImage != null) cardBackImage.sprite = uncommonBack;
            }
            else if (eventData.rarity == EventRarity.Rare)
            {
                if (cardFrame != null) cardFrame.sprite = rareFrame;
                if (cardBackImage != null) cardBackImage.sprite = rareBack;
            }
        }
        // Show panel and animate card flip start with back to front
        panel.SetActive(true);
        cardBack.SetActive(true);
        cardFront.SetActive(false);

        // cardBack.GetComponent<Image>().sprite = eventData.backcard;
        // cardFront.GetComponent<Image>().sprite = eventData.frontcard;
        // iconImage.sprite = eventData.icon;
        // nameText.text = $"{eventData.eventName}";
        // descriptionText.text = $"{eventData.description}";


        StartCoroutine(RevealProcess());
    }

    IEnumerator RevealProcess()
    {
        // // Wait for 1 second before flipping
        yield return new WaitForSeconds(1f);
        // Flip the card to reveal front
        RevealCard();
        Debug.Log("Card Revealed!");
        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(WaitPointerClick());
        ClosePanel();
        Debug.Log("Closing Panel Now...");
    }

    IEnumerator WaitPointerClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                yield break;
            }
            yield return null;
        }
    }


    // wait for player click to flip the card
    // isWaitingForInput = true;

    // Add a listener to the cardBack button to set isWaitingForInput to false when clicked
    //     Button cardBackButton = cardBack.GetComponent<Button>();
    //     void OnCardBackClicked()
    //     {
    //         isWaitingForInput = false;
    //     }
    //     cardBackButton.onClick.AddListener(OnCardBackClicked);

    //     while (isWaitingForInput)
    //     {
    //         yield return null;
    //     }

    //     // Remove the listener after input is received
    //     cardBackButton.onClick.RemoveListener(OnCardBackClicked);

    //     // Flip the card to reveal front
    //     RevealCard();
    // }

    void RevealCard()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
        // Button cardFrontButton = cardFront.GetComponent<Button>();
        // cardFrontButton.onClick.AddListener(OnCardFrontClicked);

        //Sound effect can be added here like "ฟึ่บ"
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        SavingGameLogicManager.Instance.SetIsPaused(false);
        // Time.timeScale = 1f; // Resume the game,
    }
    public void OnCardFrontClicked()
    {
        // Close the panel and trigger the event effect
        Debug.Log("Card front clicked, closing panel and triggering event effect.");
        ClosePanel();
    }
}