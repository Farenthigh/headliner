using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingRoundText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 50f;
    public float fadeDuration = 1.5f;

    public void Setup(int amount)
    {
        // โชว์ข้อความ เช่น +1 เดือน
        textMesh.text = $"+{amount} เดือน";
        
        // ใช้สีม่วงชมพูเพื่อให้เด่นและต่างจากเงิน/เวลา
        textMesh.color = new Color(1f, 0.8f, 0.1f);
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