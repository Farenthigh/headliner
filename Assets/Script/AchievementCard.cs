using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementCard : MonoBehaviour
{
    [Header("Card Structure")]
    public Transform cardRoot; // 👉 อ้างอิง CardRoot จากรูป
    public GameObject cardFace; // หน้าการ์ด (ตอนเพิ่งได้)
    public GameObject cardBack; // หลังการ์ด (รายละเอียด)
    public Button cardButton;
    
    [Header("Card UI Elements")]
    public Image iconFace;
    public TextMeshProUGUI titleBack;
    public TextMeshProUGUI descBack;
    public TextMeshProUGUI dateBack;

    [Header("VFX & Animations (จาก Event)")]
    public Image auraGlowImage; // 👉 อ้างอิง AuraGlow จากรูป
    public Transform starsGroup; // 👉 อ้างอิง StarsGroup จากรูป
    public float auraSpeed = 2f;
    public float starSpinSpeed = 30f;

    [Header("Hint Settings")]
    public GameObject clickHintText; // 👉 อ้างอิง CloseHintText จากรูป
    public TextMeshProUGUI hintTextComponent;
    public string openHintMessage = "คลิกเพื่อเปิดการ์ด";
    public string closeHintMessage = "คลิกเพื่อปิด";
    public float waitTimeBeforeShake = 3f;  
    public float shakeBurstDuration = 0.5f; 

    private bool isFlipping = false;
    private bool isShowingBack = false; // true = พลิกมาดูรายละเอียดแล้ว
    private Coroutine idleCoroutine;
    private Coroutine starsCoroutine;

    private void Start() 
    { 
        cardButton.onClick.AddListener(OnCardClicked); 
    }

    // ฟังก์ชันนี้เรียกตอนเตรียมข้อมูลก่อนการ์ดเด้งขึ้นมา
    public void SetupCard(AchievementData data)
    {
        if(iconFace != null) iconFace.sprite = data.icon;
        if(titleBack != null) titleBack.text = data.achievementName;
        if(descBack != null) descBack.text = data.description;
        if(dateBack != null) dateBack.text = data.unlockDate;

        // Reset ค่าเริ่มต้น
        cardRoot.localRotation = Quaternion.identity;
        cardFace.SetActive(true); // โชว์หน้าไอคอนก่อน
        cardBack.SetActive(false);
        isShowingBack = false;
        cardButton.interactable = false;

        if (clickHintText != null) clickHintText.SetActive(false);
        if (auraGlowImage != null) auraGlowImage.gameObject.SetActive(false);
        
        StopAllCoroutines();
    }

    // ฟังก์ชันนี้เรียกหลังจากที่แอนิเมชันเลื่อนการ์ดขึ้นมากลางจอ (ของ Manager) จบแล้ว
    public void PlayUnlockVFX()
    {
        cardButton.interactable = true;
        
        // 1. เล่นออร่า
        StartCoroutine(AuraPopUpCoroutine());
        
        // 2. หมุนดาว
        if (starsGroup != null)
        {
            starsCoroutine = StartCoroutine(SpinStarsCoroutine());
        }

        // 3. เริ่มจับเวลาสั่นไพ่และขึ้นคำใบ้
        if (hintTextComponent != null) hintTextComponent.text = openHintMessage;
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(0f));
    }

    private void OnCardClicked()
    {
        if (isFlipping) return;

        // หยุดสั่นตอนที่กำลังคลิก
        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        if (clickHintText != null) clickHintText.SetActive(false);

        if (!isShowingBack) 
        {
            StartCoroutine(FlipCoroutine()); // พลิกไปดูรายละเอียด
        }
        else 
        {
            // ถ้าดูรายละเอียดแล้วคลิกอีก จะเป็นการปิด
            StartCoroutine(FlipBackAndCloseCoroutine()); 
        }
    }

    // แอนิเมชันพลิกการ์ด 
    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        float duration = 0.4f;
        float time = 0;
        Quaternion startRot = cardRoot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 180, 0); // พลิก

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float smoothT = t * t * (3f - 2f * t);
            cardRoot.localRotation = Quaternion.Lerp(startRot, targetRot, smoothT);

            // สลับการแสดงผลตอนหมุนได้ครึ่งทาง
            if (smoothT >= 0.5f && cardFace.activeSelf)
            {
                cardFace.SetActive(false);
                cardBack.SetActive(true);
            }
            yield return null;
        }
        
        cardRoot.localRotation = targetRot;
        isShowingBack = true;
        isFlipping = false;

        // เปลี่ยนคำใบ้และเริ่มจับเวลาสั่นใหม่ (คราวนี้สั่นที่มุม 180 องศา)
        if (hintTextComponent != null) hintTextComponent.text = closeHintMessage;
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(180f));
    }

    // พลิกกลับและสั่งปิด Panel
    private IEnumerator FlipBackAndCloseCoroutine()
    {
        isFlipping = true;
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

            if (smoothT >= 0.5f && cardBack.activeSelf)
            {
                cardBack.SetActive(false);
                cardFace.SetActive(true);
            }
            yield return null;
        }
        
        cardRoot.localRotation = targetRot;
        isShowingBack = false;
        isFlipping = false;

        // หยุดดาว
        if (starsCoroutine != null) StopCoroutine(starsCoroutine);

        // สั่งให้ Manager ปิดหน้าต่างนี้
        AchievementManager.Instance.CloseUnlockPanel();
    }

    // แอนิเมชันออร่า (นำมาจาก EventDisplay)
    private IEnumerator AuraPopUpCoroutine()
    {
        if (auraGlowImage == null) yield break;

        auraGlowImage.gameObject.SetActive(true);
        auraGlowImage.transform.localScale = Vector3.one;
        Color startColor = auraGlowImage.color;
        startColor.a = 0.8f;
        auraGlowImage.color = startColor;

        float elapsed = 0f;
        float duration = 1f / auraSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t);

            float scale = Mathf.Lerp(1f, 2f, smoothT);
            auraGlowImage.transform.localScale = new Vector3(scale, scale, 1f);

            float alpha = Mathf.Lerp(startColor.a, 0f, smoothT);
            Color currentColor = auraGlowImage.color;
            currentColor.a = alpha;
            auraGlowImage.color = currentColor;

            yield return null;
        }
        auraGlowImage.gameObject.SetActive(false);
    }

    // แอนิเมชันดาวหมุนวิบวับ
    private IEnumerator SpinStarsCoroutine()
    {
        while(true)
        {
            starsGroup.Rotate(Vector3.forward * starSpinSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // แอนิเมชันสั่นเรียกความสนใจ (นำมาจาก EventDisplay)
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
                // สั่นโดยอิงจากแกน Y ปัจจุบัน (0 หรือ 180)
                cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, zRot);
                yield return null;
            }

            cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, 0);
        }
    }
}