using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Shadow shadow; // 👈 เพิ่ม

    public CharacterHover otherCharacter;
    
    private void Start()
    {
        shadow.enabled = false; // เริ่มต้นไม่ให้มีเงา
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ตัวเอง
        image.color = Color.white;
        transform.localScale = Vector3.one * 1.15f;
        shadow.enabled = true; // 👈 เปิดเงา

        // อีกตัว = เทา + ปิดเงา
        if (otherCharacter != null)
        {
            otherCharacter.SetDimmed();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisual();

        if (otherCharacter != null)
        {
            otherCharacter.ResetVisual();
        }
    }

    public void ResetVisual()
    {
        image.color = Color.white;
        transform.localScale = Vector3.one;
        shadow.enabled = false; // 👈 ปิดเงา
    }

    public void SetDimmed()
    {
        image.color = new Color(0.4f, 0.4f, 0.4f);
        transform.localScale = Vector3.one;
        shadow.enabled = false; // 👈 ปิดเงา
    }
}