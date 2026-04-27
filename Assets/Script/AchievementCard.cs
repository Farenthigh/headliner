using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementCard : MonoBehaviour
{
    [Header("Card Structure")]
    public Transform cardRoot;
    public GameObject cardFace;   // ✅ ใน Inspector: ลาก 'front' มาใส่ตรงนี้
    public GameObject cardBack;   // ✅ ใน Inspector: ลาก 'back' มาใส่ตรงนี้
    public Button cardButton;

    [Header("Card UI Elements")]
    public Image iconFace;
    public TextMeshProUGUI titleBack;
    public TextMeshProUGUI descBack;
    public TextMeshProUGUI dateBack;

    [Header("VFX & Animations")]
    public Image auraGlowImage;
    public Transform starsGroup;
    public float auraSpeed = 2f;
    public float starSpinSpeed = 30f;

    [Header("Hint Settings")]
    public GameObject clickHintText;
    public TextMeshProUGUI hintTextComponent;
    public string openHintMessage  = "คลิกเพื่อเปิดการ์ด";
    public string closeHintMessage = "คลิกเพื่อปิด";
    public float waitTimeBeforeShake = 3f;
    public float shakeBurstDuration  = 0.5f;

    private bool isFlipping    = false;
    private bool isShowingBack = false;
    private Coroutine idleCoroutine;
    private Coroutine starsCoroutine;

    private void Start()
    {
        cardButton.onClick.AddListener(OnCardClicked);
    }

    // ─── เรียกก่อนการ์ดเด้งขึ้นมา ────────────────────────────────────────────
    public void SetupCard(AchievementData data)
    {
        if (iconFace  != null) iconFace.sprite     = data.icon;
        if (titleBack != null) titleBack.text       = data.achievementName;
        if (descBack  != null) descBack.text        = data.description;
        if (dateBack  != null) dateBack.text        = data.unlockDate;

        // ✅ FIX: Pre-rotate หลังการ์ด 180° บน Y เพื่อแก้ตัวหนังสือกลับด้าน
        // เมื่อ cardRoot หมุน 180° Y → cardBack จะหมุนรวม 360° = อ่านออก
        cardBack.transform.localRotation = Quaternion.Euler(0, 180f, 0);

        // Reset state
        cardRoot.localRotation = Quaternion.identity;
        cardFace.SetActive(true);   // แสดงหน้าไอคอนก่อน
        cardBack.SetActive(false);
        isShowingBack = false;
        isFlipping    = false;
        cardButton.interactable = false;

        if (clickHintText  != null) clickHintText.SetActive(false);
        if (auraGlowImage  != null) auraGlowImage.gameObject.SetActive(false);

        StopAllCoroutines();
    }

    // ─── เรียกหลัง animation เลื่อนการ์ดขึ้นมากลางจอเสร็จ ────────────────────
    public void PlayUnlockVFX()
    {
        cardButton.interactable = true;

        StartCoroutine(AuraPopUpCoroutine());

        if (starsGroup != null)
            starsCoroutine = StartCoroutine(SpinStarsCoroutine());

        if (hintTextComponent != null)
            hintTextComponent.text = openHintMessage;

        // ✅ เริ่ม hint ทันที (ไม่รอ shake แรก) แล้วค่อย loop
        if (clickHintText != null) clickHintText.SetActive(true);
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(0f));
    }

    // ─── คลิกการ์ด ───────────────────────────────────────────────────────────
    private void OnCardClicked()
    {
        if (isFlipping) return;

        if (idleCoroutine != null) StopCoroutine(idleCoroutine);
        if (clickHintText != null) clickHintText.SetActive(false);

        if (!isShowingBack)
            StartCoroutine(FlipCoroutine());          // พลิกดูรายละเอียด
        else
            StartCoroutine(FlipBackAndCloseCoroutine()); // พลิกกลับแล้วปิด
    }

    // ─── พลิกไปด้านหลัง ──────────────────────────────────────────────────────
    private IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        float duration = 0.4f, time = 0f;
        Quaternion startRot  = cardRoot.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, 180f, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float smoothT = SmoothStep(time / duration);
            cardRoot.localRotation = Quaternion.Lerp(startRot, targetRot, smoothT);

            // สลับ panel ตอนหมุนครึ่งทาง (ผู้ชมมองไม่เห็น)
            if (smoothT >= 0.5f && cardFace.activeSelf)
            {
                cardFace.SetActive(false);
                cardBack.SetActive(true);
            }
            yield return null;
        }

        cardRoot.localRotation = targetRot;
        isShowingBack = true;
        isFlipping    = false;

        if (hintTextComponent != null) hintTextComponent.text = closeHintMessage;
        if (clickHintText     != null) clickHintText.SetActive(true);
        idleCoroutine = StartCoroutine(WaitAndShakeBurstCoroutine(180f));
    }

    // ─── พลิกกลับแล้วสั่งปิด Panel ──────────────────────────────────────────
    private IEnumerator FlipBackAndCloseCoroutine()
    {
        isFlipping = true;
        float duration = 0.4f, time = 0f;
        Quaternion startRot  = cardRoot.localRotation;
        Quaternion targetRot = Quaternion.identity;

        while (time < duration)
        {
            time += Time.deltaTime;
            float smoothT = SmoothStep(time / duration);
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
        isFlipping    = false;

        if (starsCoroutine != null) StopCoroutine(starsCoroutine);

        // ✅ แจ้ง Manager ให้ปิด และ trigger callback (เพื่อรันเนื้อเรื่องต่อ)
        AchievementManager.Instance.CloseUnlockPanel();
    }

    // ─── VFX ─────────────────────────────────────────────────────────────────
    private IEnumerator AuraPopUpCoroutine()
    {
        if (auraGlowImage == null) yield break;

        auraGlowImage.gameObject.SetActive(true);
        auraGlowImage.transform.localScale = Vector3.one;

        Color c = auraGlowImage.color;
        c.a = 0.8f;
        auraGlowImage.color = c;

        float elapsed = 0f, duration = 1f / auraSpeed;
        float startAlpha = c.a;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = SmoothStep(elapsed / duration);

            float scale = Mathf.Lerp(1f, 2f, t);
            auraGlowImage.transform.localScale = new Vector3(scale, scale, 1f);

            Color cur = auraGlowImage.color;
            cur.a = Mathf.Lerp(startAlpha, 0f, t);
            auraGlowImage.color = cur;

            yield return null;
        }
        auraGlowImage.gameObject.SetActive(false);
    }

    private IEnumerator SpinStarsCoroutine()
    {
        while (true)
        {
            starsGroup.Rotate(Vector3.forward * starSpinSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // ─── Shake + Hint loop ────────────────────────────────────────────────────
    private IEnumerator WaitAndShakeBurstCoroutine(float baseYRotation)
    {
        // ✅ รอ 1 รอบแรกก่อน แล้วค่อย shake ซ้ำ
        yield return new WaitForSeconds(waitTimeBeforeShake);

        while (true)
        {
            if (clickHintText != null) clickHintText.SetActive(true);

            float elapsed = 0f;
            while (elapsed < shakeBurstDuration)
            {
                elapsed += Time.deltaTime;
                float zRot = Mathf.Sin(elapsed * 30f) * 4f;
                cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, zRot);
                yield return null;
            }

            cardRoot.localRotation = Quaternion.Euler(0, baseYRotation, 0);
            yield return new WaitForSeconds(waitTimeBeforeShake);
        }
    }

    // ─── Utility ──────────────────────────────────────────────────────────────
    private float SmoothStep(float t) => t * t * (3f - 2f * t);
}