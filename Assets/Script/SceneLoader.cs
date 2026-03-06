using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public GameObject saving;
    public GameObject taxGameTips;
    public GameObject icon;


    void Start()
    {
        icon.SetActive(true);
        saving.SetActive(false);
        taxGameTips.SetActive(false);
    }


    public void ShowSaving()
    {
        if (saving.activeSelf) return;

        saving.SetActive(true);
        taxGameTips.SetActive(false);
    }

    public void ShowTax()
    {
        if (taxGameTips.activeSelf) return;

        taxGameTips.SetActive(true);
        saving.SetActive(false);
    }

    // กด icon เพื่อเปิด Tips
    public void OpenTips()
    {
        icon.SetActive(false);

        saving.SetActive(false);       // เปิดหน้าแรก
        taxGameTips.SetActive(true);
    }

    // กด exit เพื่อกลับไป icon
    public void ExitTips()
    {
        icon.SetActive(true);

        saving.SetActive(false);
        taxGameTips.SetActive(false);
    }
}