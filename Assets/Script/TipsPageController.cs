using UnityEngine;
using System.Collections.Generic;

public class TipsPageController : MonoBehaviour
{
    public List<GameObject> pages;
    private int currentPage = 0;

    void Start()
    {
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }
}