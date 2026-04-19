using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement; 

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI References")]
    public GameObject loadingPanel; 

    [Header("Loading Settings")]
    public float minLoadTime = 2.0f;

    // ===================================================
    // 🔴 1. ฟังก์ชันโหลดฉากแบบใช้ "ชื่อฉาก (String)"
    // ===================================================
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        Show(); 
        
        // 1. จดจำเวลาเริ่มต้น
        float startTime = Time.realtimeSinceStartup;
        
        // 2. เริ่มโหลดฉาก
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // 🔴 3. สั่งบอก Unity ว่า "โหลดเสร็จแล้วอย่าเพิ่งสลับฉากนะ รอคำสั่งก่อน!"
        operation.allowSceneActivation = false; 
        
        // 4. รอจนกว่า: (ฉากจะโหลดไปถึง 90% ขึ้นไป) และ (เวลาผ่านไปเกิน minLoadTime แล้ว)
        while (operation.progress < 0.9f || (Time.realtimeSinceStartup - startTime) < minLoadTime)
        {
            yield return null;
        }

        // 🔴 5. พอเงื่อนไขครบ (เวลาครบ + โหลดเสร็จ) ก็อนุญาตให้สลับฉากได้เลย!
        operation.allowSceneActivation = true;
    }

    // ===================================================
    // 🔴 2. ฟังก์ชันโหลดฉากแบบใช้ "ลำดับตัวเลข (Int)" (เพิ่มใหม่แก้ Error!)
    // ===================================================
    public void LoadScene(int sceneBuildIndex)
    {
        StartCoroutine(LoadSceneCoroutineByIndex(sceneBuildIndex));
    }

    private IEnumerator LoadSceneCoroutineByIndex(int sceneBuildIndex)
    {
        Show(); 
        float startTime = Time.realtimeSinceStartup;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        operation.allowSceneActivation = false; 
        
        while (operation.progress < 0.9f || (Time.realtimeSinceStartup - startTime) < minLoadTime)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;
    }
    // ===================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Hide(); 
    }

    public void Show()
    {
        loadingPanel.SetActive(true);
    }

    public void Hide()
    {
        loadingPanel.SetActive(false);
    }

    public void FakeLoad(float durationInSeconds, Action onComplete = null)
    {
        StartCoroutine(FakeLoadCoroutine(durationInSeconds, onComplete));
    }
    // 🔴 เพิ่มฟังก์ชันนี้เข้ามาสำหรับให้ปุ่มใน Unity มองเห็น
    public void TestFakeLoad(float duration)
    {
        // 🔴 บังคับให้มันเรียกผ่าน Instance ตัวจริงที่รอดชีวิตเสมอ!
        if (Instance != null)
        {
            Instance.FakeLoad(duration, null);
        }
    }

    private IEnumerator FakeLoadCoroutine(float duration, Action onComplete)
    {
        Show(); 
        yield return new WaitForSecondsRealtime(duration); 
        Hide(); 
        onComplete?.Invoke(); 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Hide(); 
    }
}