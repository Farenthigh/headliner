using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI dateText;
    public GameObject lockedOverlay;

    private AchievementData myData;

    public void SetupSlot(AchievementData data)
    {
        myData = data;

        nameText.text = data.achievementName;
        descriptionText.text = data.description;
        iconImage.sprite = data.icon;

        if (data.isUnlocked)
        {
            lockedOverlay.SetActive(false);
            dateText.text = "unlocked : " + data.unlockDate;
        }
        else
        {
            lockedOverlay.SetActive(true);
            dateText.text = "locked";
        }
    }

    public void OnClickSlot()
    {
        if (myData == null) return;

        if (myData.isUnlocked)
        {
            AchievementManager.Instance.ShowPopup(myData);
        }
        else
        {
            Debug.Log("ยังไม่ปลดล็อก: " + myData.description);
        }
    }
}