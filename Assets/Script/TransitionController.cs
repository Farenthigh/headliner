using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TransitionController : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public Image fadeOverlayUI;
    public float fadeSpeed = 0.5f;

    [Header("Character Transition Settings")]
    public float characterFadeSpeed = 0.3f;

    // ใช้ Dictionary เพื่อจัดการกรณีที่มีตัวละครโผล่มาพร้อมกันหลายตัว จะได้ไม่บั๊กทับกัน
    private Dictionary<Image, Coroutine> activeCharacterFades = new Dictionary<Image, Coroutine>();

    // ฟังก์ชันสั่ง Fade เปลี่ยนฉาก (รับคำสั่งมาจาก StoryManager)
    public void PlaySceneFade(Action onMidFade, Action onComplete)
    {
        StartCoroutine(FadeAndChangePageRoutine(onMidFade, onComplete));
    }

    private IEnumerator FadeAndChangePageRoutine(Action onMidFade, Action onComplete)
    {
        // 1. Fade Out (จอมืดลง)
        if (fadeOverlayUI != null)
        {
            fadeOverlayUI.gameObject.SetActive(true);
            float time = 0;
            while (time < fadeSpeed)
            {
                time += Time.deltaTime;
                fadeOverlayUI.color = new Color(0, 0, 0, Mathf.Clamp01(time / fadeSpeed));
                yield return null;
            }
            fadeOverlayUI.color = new Color(0, 0, 0, 1);
        }

        // 2. จังหวะที่จอมืดสนิท ให้ StoryManager อัปเดต UI (เปลี่ยนภาพ/ข้อความ)
        onMidFade?.Invoke();

        yield return new WaitForSeconds(0.2f); // หน่วงเวลาให้พักสายตา

        // 3. Fade In (สว่างขึ้น)
        if (fadeOverlayUI != null)
        {
            float time = 0;
            while (time < fadeSpeed)
            {
                time += Time.deltaTime;
                fadeOverlayUI.color = new Color(0, 0, 0, 1f - Mathf.Clamp01(time / fadeSpeed));
                yield return null;
            }
            fadeOverlayUI.color = new Color(0, 0, 0, 0);
            fadeOverlayUI.gameObject.SetActive(false);
        }

        // 4. เสร็จสมบูรณ์ คืนสิทธิให้กดปุ่ม Next ได้
        onComplete?.Invoke();
    }

    // ฟังก์ชันสั่ง Fade ตัวละครโผล่มา
    public void FadeInCharacter(Image characterImage)
    {
        if (activeCharacterFades.ContainsKey(characterImage) && activeCharacterFades[characterImage] != null)
        {
            StopCoroutine(activeCharacterFades[characterImage]);
        }
        
        activeCharacterFades[characterImage] = StartCoroutine(FadeInCharacterRoutine(characterImage));
    }

    private IEnumerator FadeInCharacterRoutine(Image characterImage)
    {
        Color c = characterImage.color;
        c.a = 0f;
        characterImage.color = c;

        float time = 0;
        while (time < characterFadeSpeed)
        {
            time += Time.deltaTime;
            c.a = Mathf.Clamp01(time / characterFadeSpeed);
            characterImage.color = c;
            yield return null;
        }

        c.a = 1f;
        characterImage.color = c;
    }
}