using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour
{
    [SerializeField] private Image characterLeft;
    [SerializeField] private Image characterCenter;
    [SerializeField] private Image characterRight;

    public void ShowCharacters(Sprite leftSprite, Sprite centerSprite, Sprite rightSprite)
    {
        SetCharacter(characterLeft, leftSprite);
        SetCharacter(characterCenter, centerSprite);
        SetCharacter(characterRight, rightSprite);
    }

    private void SetCharacter(Image image, Sprite sprite)
    {
        if (sprite == null) return;

        image.sprite = sprite;

        // ขนาดจริงของรูป
        float nativeWidth  = sprite.rect.width;
        float nativeHeight = sprite.rect.height;

        // กำหนดขนาดสูงสุดที่ยอมให้แสดง
        float maxWidth  = 400f;  // ปรับตามที่ต้องการ
        float maxHeight = 500f;  // ปรับตามที่ต้องการ

        // คำนวณ scale ที่พอดีโดยไม่บิดเบี้ยว
        float scale = Mathf.Min(maxWidth / nativeWidth, maxHeight / nativeHeight, 1f);
        // 1f = ถ้ารูปเล็กกว่า max ก็แสดงขนาดจริง ไม่ขยาย

        image.rectTransform.sizeDelta = new Vector2(nativeWidth * scale, nativeHeight * scale);
    }
}