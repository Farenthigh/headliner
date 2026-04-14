using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EventDisplay : MonoBehaviour
{
    public static EventDisplay Instance;

    [Header("UI Objects")]
    public GameObject panel;
    public Transform cardRoot; 
    public GameObject cardBack;
    public GameObject cardFront;

    [Header("UI Elements")]
    public Image cardFrame;
    public Image cardBackImage;
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("Rarity Sprites")]
    public Sprite commonFrame; public Sprite uncommonFrame; public Sprite rareFrame;
    public Sprite commonBack; public Sprite uncommonBack; public Sprite rareBack;

    private bool isFlipped = false;
    private bool isAnimating = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        panel.SetActive(false);

        if (cardFront != null)
            cardFront.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void ShowEvent(Event eventData)
    {
        UpdateRarityUI(eventData.rarity);

        if (nameText != null) nameText.text = eventData.eventName;
        if (descriptionText != null) descriptionText.text = eventData.description;
        if (iconImage != null) iconImage.sprite = eventData.icon;

        StopAllCoroutines(); 
        cardRoot.rotation = Quaternion.identity;
        cardBack.SetActive(true);
        cardFront.SetActive(false);
        
        isFlipped = false;
        isAnimating = false;
        panel.SetActive(true);
    }

    private void UpdateRarityUI(EventRarity rarity)
    {
        switch (rarity)
        {
            case EventRarity.Common:
                cardFrame.sprite = commonFrame; cardBackImage.sprite = commonBack; break;
            case EventRarity.Uncommon:
                cardFrame.sprite = uncommonFrame; cardBackImage.sprite = uncommonBack; break;
            case EventRarity.Rare:
                cardFrame.sprite = rareFrame; cardBackImage.sprite = rareBack; break;
        }
    }

    public void OnCardClicked()
    {
        if (isAnimating) return;

        if (!isFlipped)
        {
            StartCoroutine(FlipCoroutine());
        }
        else
        {
            Debug.Log("Card is already flipped, closing now...");
            ClosePanel();
        }
    }

    private IEnumerator FlipCoroutine()
    {
        isAnimating = true;
        float duration = 0.4f; 
        float time = 0;
        Quaternion startRot = cardRoot.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 180, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            float smoothT = t * t * (3f - 2f * t); 
            cardRoot.rotation = Quaternion.Lerp(startRot, targetRot, smoothT);

            if (smoothT >= 0.5f && cardBack.activeSelf)
            {
                cardBack.SetActive(false);
                cardFront.SetActive(true);
            }
            yield return null;
        }
        
        cardRoot.rotation = targetRot;
        isFlipped = true;
        isAnimating = false;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        if (SavingGameLogicManager.Instance != null)
            SavingGameLogicManager.Instance.SetIsPaused(false);
    }
}