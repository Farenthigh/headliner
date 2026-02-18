using UnityEngine;
using System.Collections.Generic;

public class TipsPageController : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;

    private int currentPage = 0;
    private bool isValid = false;

    private void Awake()
    {
        isValid = ValidatePages();

        if (!isValid)
        {
            enabled = false; // ปิด script เลยถ้า config ผิด
            return;
        }

        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage >= pages.Count - 1)
            return;

        currentPage++;
        ShowPage(currentPage);
    }

    public void PreviousPage()
    {
        if (currentPage <= 0)
            return;

        currentPage--;
        ShowPage(currentPage);
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    private bool ValidatePages()
    {
        if (pages == null || pages.Count == 0)
        {
            Debug.LogError("TipsPageController: No pages assigned.", this);
            return false;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] == null)
            {
                Debug.LogError($"TipsPageController: Page at index {i} is null.", this);
                return false;
            }
        }

        return true;
    }
}
