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

    [Header("Rarity Sprites")]
    public Sprite commonFrame;
    public Sprite uncommonFrame;
    public Sprite rareFrame;
    public Image cardFrame;

    [Header("UI Elements")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private bool isWaitingForInput = false;

    private void Awake() => Instance = this;

    public void ShowEvent(Event eventData)
    {
        // Set Front card details but hide it
        nameText.text = eventData.eventName;
        descriptionText.text = eventData.description;
        iconImage.sprite = eventData.icon;

        if(cardFrame != null)
        {
            if(eventData.rarity == EventRarity.Common)
            {
                cardFrame.sprite = commonFrame;
            }
            else if(eventData.rarity == EventRarity.Uncommon)
            {
                cardFrame.sprite = uncommonFrame;
            }
            else if(eventData.rarity == EventRarity.Rare)
            {
                cardFrame.sprite = rareFrame;
            }
        }

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
        while (isWaitingForInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isWaitingForInput = false;
            }
            yield return null;
        }
        // Flip the card to reveal front
        RevealCard();
    }

    void RevealCard()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
        //Sound effect can be added here like "ฟึ่บ"
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}