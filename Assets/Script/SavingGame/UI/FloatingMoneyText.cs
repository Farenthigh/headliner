using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingMoneyText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 50f;     // ความเร็วในการลอยขึ้น
    public float fadeDuration = 1.5f; // เวลาที่ใช้ก่อนจะจางหายไป

    public void Setup(float amount)
    {
        if (amount > 0)
        {
            textMesh.text = $"+{amount:N0}";
            textMesh.color = new Color(0.2f, 0.8f, 0.2f); // สีเขียวสว่าง
        }
        else
        {
            textMesh.text = $"{amount:N0}"; // ค่ามันติดลบอยู่แล้ว เลยไม่ต้องใส่ - เพิ่ม
            textMesh.color = new Color(0.9f, 0.2f, 0.2f); // สีแดง
        }
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        float time = 0;
        Color startColor = textMesh.color;

        while (time < fadeDuration)
        {
            // ใช้ unscaledDeltaTime เพื่อให้ลอยได้แม้เกมจะ Pause อยู่ (ตอนเปิดการ์ด)
            time += Time.unscaledDeltaTime; 
            
            // 1. เลื่อนตำแหน่งขึ้น
            transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;
            
            // 2. ค่อยๆ จางหาย (ลดค่า Alpha)
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
        
        // ทำลาย GameObject ทิ้งเมื่อแอนิเมชันจบเพื่อไม่ให้รกเครื่อง
        Destroy(gameObject); 
    }
}