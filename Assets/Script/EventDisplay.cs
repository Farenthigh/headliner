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

    // 🔴 เพิ่มตัวแปรสำหรับแสงออร่า
    [Header("Aura VFX")]
    public Image auraGlowImage; 
    public float auraSpeed = 2f; // ความเร็วในการขยายและจางหาย

    [Header("UI Elements")]
    public Image cardFrame;
    public Image cardBackImage;
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("Hint Settings")]
    public GameObject clickHintText; 
    public TextMeshProUGUI hintTextComponent;
    public string openHintMessage = "คลิกเพื่อเปิดการ์ด";
    public string closeHintMessage = "คลิกเพื่อปิด";
    public float waitTimeBeforeShake = 3f;  
    public float shakeBurstDuration = 0.5f; 

    [Header("Rarity Sprites")]
    public Sprite commonFrame; public Sprite uncommonFrame; public Sprite rareFrame;
    public Sprite commonBack; public Sprite uncommonBack; public Sprite rareBack;

    private bool isFlipped = false;
    private bool isAnimating = false;
    private Coroutine idleCoroutine; 

    private Event currentEventData;

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
            
        if (clickHintText != null) clickHintText.SetActive(false);
        
        // ซ่อนออร่าไว้ตอนเริ่มเกม
        if (auraGlowImage != null) auraGlowImage.gameObject.SetActive(false);
    }

    public void ShowEvent(Event eventData)
    {
        // 🔴 1. เก็บข้อมูล Event ไว้ในตัวแปร
        currentEventData = eventData; 

        UpdateRarityUI(eventData.rarity);

        if (nameText != null) nameText.text = eventData.eventName;
        if (descriptionText != null) descriptionText.text = eventData.description;
        if (iconImage != null) iconImage.sprite = eventData.icon;
        
        // 🔴 แสดงตัวเลขบนหน้าการ์ด (ถ้าคุณมีช่อง amount ใน Class Event)
        // if (amountText != null) amountText.text = eventData.amount.ToString();

        StopAllCoroutines(); 
        if (clickHintText != null) clickHintText.SetActive(false);

        cardRoot.localRotation = Quaternion.identity;
        cardBack.SetActive(true);
        cardFront.SetActive(false);
        
        isFlipped = false;
        isAnimating = false;
        panel.SetActive(true);

        StartCoroutine(AuraPopUpCoroutine());

        if (hintTextComponent != null) hintTextComponent.text = openHintMessage;
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(0f));
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

    // 🔴 Coroutine ใหม่: ทำแอนิเมชันแสงออร่าเด้งขึ้นมาและจางหาย
    private IEnumerator AuraPopUpCoroutine()
    {
        if (auraGlowImage == null) yield break;

        auraGlowImage.gameObject.SetActive(true);
        
        // 1. ตั้งค่าเริ่มต้น: ขนาดเล็ก, สีทึบ
        auraGlowImage.transform.localScale = Vector3.one; // ขนาดปกติของการ์ด
        Color startColor = auraGlowImage.color;
        startColor.a = 0.8f; // เริ่มต้นให้เห็นชัดหน่อย (0-1)
        auraGlowImage.color = startColor;

        float elapsed = 0f;
        float duration = 1f / auraSpeed; // คำนวณเวลาจากความเร็ว

        // 2. ค่อยๆ ขยายขนาดและจางหาย
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // 🔴 ใช้ unscaledDeltaTime เผื่อเกม Pause อยู่
            float t = elapsed / duration;
            
            // ใช้ SmoothStep เพื่อให้แอนิเมชันดูนุ่มนวล
            float smoothT = t * t * (3f - 2f * t);

            // ขยายขนาดจาก 1x เป็น 2x (ปรับเลข 2f ได้ถ้าอยากให้ขยายใหญ่ขึ้น)
            float scale = Mathf.Lerp(1f, 2f, smoothT);
            auraGlowImage.transform.localScale = new Vector3(scale, scale, 1f);

            // จางหายจาก Alpha 0.8f เป็น 0
            float alpha = Mathf.Lerp(startColor.a, 0f, smoothT);
            Color currentColor = auraGlowImage.color;
            currentColor.a = alpha;
            auraGlowImage.color = currentColor;

            yield return null;
        }

        // 3. ทำเสร็จแล้ว ซ่อน GameObject เพื่อประหยัดเครื่อง
        auraGlowImage.gameObject.SetActive(false);
    }

    public void OnCardClicked()
    {
        if (isAnimating) return;

        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        if (clickHintText != null) clickHintText.SetActive(false);

        if (!isFlipped) StartCoroutine(FlipCoroutine());
        else StartCoroutine(FlipBackAndCloseCoroutine());
    }

    private IEnumerator FlipCoroutine()
    {
        isAnimating = true;
        float duration = 0.4f; 
        float time = 0;
        Quaternion startRot = cardRoot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 180, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float smoothT = t * t * (3f - 2f * t); 
            cardRoot.localRotation = Quaternion.Lerp(startRot, targetRot, smoothT);

            if (smoothT >= 0.5f && cardBack.activeSelf)
            {
                cardBack.SetActive(false);
                cardFront.SetActive(true);
            }
            yield return null;
        }
        
        cardRoot.localRotation = targetRot;
        isFlipped = true;
        isAnimating = false;

        // 🔴 2. จุดสำคัญ: เมื่อพลิกการ์ดเสร็จแล้ว ให้สั่งเพิ่ม/ลดเงินที่นี่!
        ApplyEventMoneyEffect();

        if (hintTextComponent != null) hintTextComponent.text = closeHintMessage;
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(180f));
    }

    // 🔴 3. ฟังก์ชันสำหรับคำนวณเงินจาก Event
    private void ApplyEventMoneyEffect()
    {
        if (currentEventData != null)
        {
            currentEventData.TriggerEvent(); 
        }
    }

    private IEnumerator WaitAndShakeBurstCoroutine(float baseYRotation)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTimeBeforeShake);
            if (clickHintText != null) clickHintText.SetActive(true);

            float elapsed = 0f;
            float shakeSpeed = 30f; 
            float shakeAngle = 4f;

            while (elapsed < shakeBurstDuration)
            {
                elapsed += Time.deltaTime;
                float zRot = Mathf.Sin(elapsed * shakeSpeed) * shakeAngle;
                cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, zRot);
                yield return null;
            }

            cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, 0);
        }
    }

    private IEnumerator FlipBackAndCloseCoroutine()
    {
        isAnimating = true;
        float duration = 0.4f; 
        float time = 0;
        Quaternion startRot = cardRoot.localRotation;
        Quaternion targetRot = Quaternion.identity; 

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float smoothT = t * t * (3f - 2f * t); 
            cardRoot.localRotation = Quaternion.Lerp(startRot, targetRot, smoothT);

            if (smoothT >= 0.5f && cardFront.activeSelf)
            {
                cardFront.SetActive(false);
                cardBack.SetActive(true);
            }
            yield return null;
        }
        
        cardRoot.localRotation = targetRot;
        isFlipped = false;
        isAnimating = false;

        ClosePanel();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        if (SavingGameLogicManager.Instance != null)
            SavingGameLogicManager.Instance.SetIsPaused(false);
    }
}