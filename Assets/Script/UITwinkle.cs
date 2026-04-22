using UnityEngine;
using System.Collections;

public class UITwinkle : MonoBehaviour
{
    [Header("Twinkle Settings")]
    public float minScale = 0.5f;     // ขนาดเล็กสุด
    public float maxScale = 1.2f;     // ขนาดใหญ่สุด
    public float speed = 1.5f;        // ความเร็วในการกระพริบ (ย่อ-ขยาย)
    public float rotationSpeed = 30f; // ความเร็วในการหมุนองศา (ใส่ 0 ถ้าไม่อยากให้หมุน)

    [Header("Randomize")]
    public bool randomStart = true;   // ให้ดาวแต่ละดวงกระพริบไม่พร้อมกัน

    private float timeOffset;

    private void Awake()
    {
        // สุ่มจังหวะเริ่มต้น จะได้ดูเป็นธรรมชาติ ไม่กระพริบพร้อมกันเป๊ะๆ
        if (randomStart)
        {
            timeOffset = Random.Range(0f, 10f);
        }
    }

    private void Update()
    {
        // 🔴 เปลี่ยนมาใช้ Time.unscaledDeltaTime 
        transform.Rotate(0, 0, rotationSpeed * Time.unscaledDeltaTime);

        // 🔴 เปลี่ยนมาใช้ Time.unscaledTime
        float wave = Mathf.Sin((Time.unscaledTime + timeOffset) * speed); 
        float normalizedWave = (wave + 1f) / 2f; 
        
        float currentScale = Mathf.Lerp(minScale, maxScale, normalizedWave);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}