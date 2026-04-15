using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudTransition : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup transitionCanvasGroup; 
    public RectTransform cloudGroupTransform; 

    [Header("Settings")]
    public float zoomInTime = 0.8f;   // เวลาตอนเมฆวิ่งมาบังจอ
    public float zoomOutTime = 0.8f;  // เวลาตอนเมฆวิ่งออกโชว์แมพ
    public Vector3 farScale = new Vector3(5f, 5f, 1f); // ขนาดเมฆตอนอยู่ไกล (ใหญ่ทะลุจอ)
    public Vector3 coverScale = new Vector3(1f, 1f, 1f); // ขนาดเมฆตอนบังจอพอดี

    public void StartTransitionAndLoad(string sceneToLoad)
    {
        // แก้เรื่องโดน Destroy: ต้องดึงตัวเองออกจาก Parent ก่อน และห้ามมีใครเป็นพ่อ
        transform.SetParent(null); 
        DontDestroyOnLoad(gameObject); 
        StartCoroutine(ExecuteFullTransition(sceneToLoad));
    }

    private IEnumerator ExecuteFullTransition(string sceneName)
    {
        // --- เตรียมหน้าจอ ---
        transitionCanvasGroup.gameObject.SetActive(true);
        transitionCanvasGroup.blocksRaycasts = true;
        transitionCanvasGroup.alpha = 1f;

        // 1. [ขาออก] เมฆซูมจากใหญ่ (farScale) ลงมาบังจอพอดี (coverScale)
        // ฟีลลิ่งจะเหมือนเรากำลังบินถอยห่างจากหน้า Home แล้วเมฆวิ่งมาบัง
        float time = 0;
        while (time < zoomInTime)
        {
            time += Time.deltaTime;
            float t = time / zoomInTime;
            // ใช้ SmoothStep ให้จังหวะนุ่มนวล
            float smoothT = t * t * (3f - 2f * t);
            cloudGroupTransform.localScale = Vector3.Lerp(farScale, coverScale, smoothT);
            yield return null;
        }
        cloudGroupTransform.localScale = coverScale;

        // 2. [เปลี่ยนฉาก] ทำตอนเมฆบังมิดแล้ว
        if (!string.IsNullOrEmpty(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone) yield return null;
        }

        // รอจังหวะสั้นๆ ให้ฉากใหม่นิ่ง (0.2 วิ)
        yield return new WaitForSeconds(0.2f);

        // 3. [ขาเข้า] เมฆซูมจากบังจอพอดี (coverScale) ขยายใหญ่ออกไป (farScale) 
        // พร้อมค่อยๆ จางหาย (Fade Out) เพื่อโชว์หน้า SelectMap
        time = 0;
        while (time < zoomOutTime)
        {
            time += Time.deltaTime;
            float t = time / zoomOutTime;
            float smoothT = t * t * (3f - 2f * t);

            cloudGroupTransform.localScale = Vector3.Lerp(coverScale, farScale, smoothT);
            transitionCanvasGroup.alpha = 1f - smoothT;
            yield return null;
        }

        // --- จบงาน ---
        transitionCanvasGroup.alpha = 0;
        transitionCanvasGroup.blocksRaycasts = false;
        Destroy(gameObject); // ทำลายตัวเองทิ้งเมื่อจบงานเพื่อคืน Memory
    }
}