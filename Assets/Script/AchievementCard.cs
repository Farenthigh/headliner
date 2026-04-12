using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementCard : MonoBehaviour
{
    public GameObject cardFace;
    public GameObject cardBack;
    public Button cardButton;
    
    public Image iconFace;
    public TextMeshProUGUI titleBack;
    public TextMeshProUGUI descBack;
    public TextMeshProUGUI dateBack;

    private bool isFlipping = false;
    private bool isShowingBack = false;

    private void Start() 
    { 
        cardButton.onClick.AddListener(FlipCard); 
    }

    public void SetupCard(AchievementData data)
    {
        if(iconFace != null) iconFace.sprite = data.icon;
        if(titleBack != null) titleBack.text = data.achievementName;
        if(descBack != null) descBack.text = data.description;
        if(dateBack != null) dateBack.text = data.unlockDate;

        transform.rotation = Quaternion.identity;
        cardFace.SetActive(true);
        cardBack.SetActive(false);
        isShowingBack = false;
        cardButton.interactable = false;
    }

    public void EnableInteraction() 
    { 
        cardButton.interactable = true; 
    }

    private void FlipCard()
    {
        if (isFlipping) return;
        StartCoroutine(FlipCoroutine());
    }

    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        float duration = 0.3f;
        float time = 0;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = isShowingBack ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            if (t >= 0.5f)
            {
                cardFace.SetActive(isShowingBack);
                cardBack.SetActive(!isShowingBack);
            }
            yield return null;
        }
        transform.rotation = targetRot;
        isShowingBack = !isShowingBack;
        isFlipping = false;
    }
}