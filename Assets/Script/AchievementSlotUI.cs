using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementSlotUI : MonoBehaviour
{
    [Header("Card Side")]
    public GameObject front;
    public GameObject back;

    [Header("Front UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI descriptionText;

    private AchievementData myData;

    public void SetupSlot(AchievementData data)
    {
        myData = data;

        // 👉 ตั้งค่าข้อมูลหน้าไพ่
        nameText.text = data.achievementName;
        iconImage.sprite = data.icon;
        descriptionText.text = data.description;

        if (data.isUnlocked)
        {
            // ✅ ปลดล็อก → โชว์หน้าไพ่
            front.SetActive(true);
            back.SetActive(false);

            dateText.text = string.IsNullOrEmpty(data.unlockDate) 
                ? "Unlocked" 
                : "Unlocked: " + data.unlockDate;
        }
        else
        {
            // 🔒 ยังไม่ปลดล็อก → โชว์หลังไพ่
            front.SetActive(false);
            back.SetActive(true);

            dateText.text = "";
        }
    }

    public void OnClickSlot()
    {
        if (myData.isUnlocked)
        {
            AchievementManager.Instance.ShowPopup(myData);
        }
        else
        {
            AchievementManager.Instance.ShowHintPopup(myData);
        }
    }
}