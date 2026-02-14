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
                if(cardFrame != null) cardFrame.sprite = commonFrame;
                if(cardBackImage != null) cardBackImage.sprite = commonBack;
            }
            else if(eventData.rarity == EventRarity.Uncommon)
            {
                if(cardFrame != null) cardFrame.sprite = uncommonFrame;
                if(cardBackImage != null) cardBackImage.sprite = uncommonBack;
            }
            else if(eventData.rarity == EventRarity.Rare)
            {
                if(cardFrame != null) cardFrame.sprite = rareFrame;
                if(cardBackImage != null) cardBackImage.sprite = rareBack;
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
        yield return StartCoroutine(WaitPointerClick());
        // Flip the card to reveal front
        RevealCard();
        Debug.Log("Card Revealed!");

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(WaitPointerClick());
        ClosePanel();
        Debug.Log("Closing Panel Now...");
    }

    IEnumerator WaitPointerClick(){
        while(true){
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)){
                yield break;
            }
            yield return null;
        }
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
        Time.timeScale = 1f; // Resume the game
    }
}