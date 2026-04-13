using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudTransition : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup transitionCanvasGroup; // ตัวคุมจางหายทั้งหน้าจอ (เอาไว้ปิดตอนจบงาน)
    public RectTransform cloudGroupTransform; // ตัวคุมซูมเมฆ
    public CanvasGroup cloudCanvasGroup;      // ตัวคุมจางหายเมฆอย่างเดียว (เพิ่ม Canvas Group ที่ตัวเมฆ)
    public RectTransform dragonTransform;    // *** รูปมังกร (ขาลากมาใส่ช่องนี้)

    [Header("Cloud Settings")]
    public float zoomInTime = 1.0f;   // เวลาเมฆซูมมาบังหน้า Home
    public float zoomOutTime = 1.0f;  // เวลาเมฆซูมออกเปิดหน้า SelectMap
    public Vector3 farScale = new Vector3(5f, 5f, 1f); // ขนาดตอนอยู่ไกล
    public Vector3 coverScale = new Vector3(1f, 1f, 1f); // ขนาดตอนบังจอ

    [Header("Dragon Settings")]
    // +++ เพิ่มการตั้งค่ามังกรบิน +++
    public float dragonFlyTime = 1.5f;   // เวลามังกรบินผ่าน
    public Vector2 dragonStartPos = new Vector2(-1500, 300); // จุดสตาร์ท (นอกจอซ้าย)
    public Vector2 dragonEndPos = new Vector2(1500, -300);   // จุดจบ (นอกจอขวา)

    public void StartTransitionAndLoad(string sceneToLoad)
    {
        // แก้เรื่องโดน Destroy: ดึงตัวเองออกจาก Parent ก่อน และห้ามมีใครเป็นพ่อ
        transform.SetParent(null); 
        DontDestroyOnLoad(gameObject); 
        StartCoroutine(ExecuteCookieRunLikeTransition(sceneToLoad));
    }

    private IEnumerator ExecuteCookieRunLikeTransition(string sceneName)
    {
        // 0. เตรียมหน้าจอ (เมฆบังมิด + บล็อกการคลิก)
        transitionCanvasGroup.gameObject.SetActive(true);
        transitionCanvasGroup.blocksRaycasts = true;
        transitionCanvasGroup.alpha = 1f;
        cloudCanvasGroup.alpha = 1f; // เมฆทึบแสง
        dragonTransform.gameObject.SetActive(false); // ซ่อนมังกรไว้ก่อน

        // 1. [ขาออก] เมฆซูมจากใหญ่มาบังจอ
        float time = 0;
        while (time < zoomInTime)
        {
            time += Time.deltaTime;
            float t = time / zoomInTime;
            float smoothT = t * t * (3f - 2f * t);
            cloudGroupTransform.localScale = Vector3.Lerp(farScale, coverScale, smoothT);
            yield return null;
        }
        cloudGroupTransform.localScale = coverScale;

        // 2. [เปลี่ยนฉาก] ทำตอนเมฆบังมิด
        if (!string.IsNullOrEmpty(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone) yield return null;
        }

        // รอจังหวะสั้นๆ ให้ฉากใหม่นิ่ง (0.2 วิ)
        yield return new WaitForSeconds(0.2f);

        // 3. [ขาเข้า] จังหวะเปิดแมพ: เมฆค่อยๆ ซูมออก + จางหายโชว์แมพ SelectMap
        time = 0;
        while (time < zoomOutTime)
        {
            time += Time.deltaTime;
            float t = time / zoomOutTime;
            float smoothT = t * t * (3f - 2f * t);

            // เมฆขยายใหญ่ขึ้นจนทะลุจอ
            cloudGroupTransform.localScale = Vector3.Lerp(coverScale, farScale, smoothT);
            // *** ไฮไลต์: จางแค่เมฆกลุ่มนี้กลุ่มเดียว เพื่อเผยแมพ แต่ไม่บล็อกหน้าจอทั้งหมด ***
            cloudCanvasGroup.alpha = 1f - smoothT;
            yield return null;
        }
        cloudCanvasGroup.alpha = 0; // เมฆหายไปหมดแล้ว ตอนนี้เห็นแมพใหม่ชัดเจน

        // ----------------------------------------------------------------------------------
        // +++ เพิ่มคิวแอนิเมชันลำดับสุดท้าย: มังกรบินต้อนรับสู่แมพใหม่! +++
        // ----------------------------------------------------------------------------------
        
        // เตรียมพร้อมมังกร (เอามันมาอยู่จุดสตาร์ท และสั่งเปิดใช้งาน)
        dragonTransform.anchoredPosition = dragonStartPos;
        dragonTransform.gameObject.SetActive(true);

        time = 0;
        while (time < dragonFlyTime)
        {
            time += Time.deltaTime;
            float percent = time / dragonFlyTime;
            // ใช้ SmoothStep ให้มังกรโฉบผ่านอย่างนุ่มนวล
            float smoothPercent = percent * percent * (3f - 2f * percent);
            dragonTransform.anchoredPosition = Vector2.Lerp(dragonStartPos, dragonEndPos, smoothPercent);
            yield return null;
        }
        // ล็อกมังกรที่จุดจบ และซ่อนมันทิ้งเพื่อคืน Memory
        dragonTransform.anchoredPosition = dragonEndPos;
        dragonTransform.gameObject.SetActive(false);

        // ----------------------------------------------------------------------------------

        // 4. จบงาน: ปลดล็อกหน้าจอให้กดปุ่มได้ และทำลาย Canvas โหลดฉากทิ้ง
        transitionCanvasGroup.blocksRaycasts = false;
        Destroy(gameObject); 
    }
}