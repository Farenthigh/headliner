using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public GameObject saving;
    public GameObject taxGameTips;
    public GameObject icon;
    public GameObject bg;
    public TipsPageController tipsPageController;
    public int defaultPageToOpen = 0;

    void Start()
    {
        bg.SetActive(false);
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

    public void OpenTips()
    {
        bg.SetActive(true);
        saving.SetActive(false);
        taxGameTips.SetActive(true);

        if (tipsPageController != null)
            tipsPageController.GoToPage(defaultPageToOpen);
    }

    public void ExitTips()
    {
        bg.SetActive(false);
        taxGameTips.SetActive(false);
    }
}