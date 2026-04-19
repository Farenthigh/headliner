using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSpinner : MonoBehaviour
{
    [Header("Spin Settings")]
    public float spinSpeed = 200f; // ความเร็วในการหมุน (องศาต่อวินาที)
    public bool spinClockwise = true;

    private void Update()
    {
        // ใช้ Time.unscaledDeltaTime เพื่อให้มังกรยังหมุนได้ แม้ว่าเกมจะถูก Pause อยู่ (Time.timeScale = 0)
        float direction = spinClockwise ? -1f : 1f;
        transform.Rotate(0, 0, spinSpeed * direction * Time.unscaledDeltaTime);
    }
}