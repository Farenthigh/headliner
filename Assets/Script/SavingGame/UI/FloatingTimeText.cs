using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingTimeText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 50f;     
    public float fadeDuration = 1.5f; 

    public void Setup(int timeAmount)
    {
        // ในระบบของคุณ timeAmount ที่ส่งมาถ้า "ติดลบ" แปลว่า "ได้เวลาเพิ่ม"
        if (timeAmount < 0) 
        {
            textMesh.text = $"+{Mathf.Abs(timeAmount)} วิ"; // โชว์เป็นบวก
            textMesh.color = new Color(0.2f, 0.9f, 0.2f);
        }
        else if (timeAmount > 0)
        {
            textMesh.text = $"-{Mathf.Abs(timeAmount)} วิ"; // โชว์เป็นลบ
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
            time += Time.unscaledDeltaTime; 
            transform.position += Vector3.up * moveSpeed * Time.unscaledDeltaTime;
            
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }
        Destroy(gameObject); 
    }
}