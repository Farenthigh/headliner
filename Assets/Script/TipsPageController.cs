using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TipsPageController : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private float flipDuration = 0.4f;

    private int currentSpread = 0;
    private int totalSpreads => Mathf.CeilToInt(pages.Count / 2f);
    private bool isFlipping = false;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private void Start()
    {
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PreviousPage);
    }

    private void Awake()
    {
        UpdateSpreadVisibility();
    }

    public void GoToPage(int pageIndex)
    {
        int spread = pageIndex / 2;
        if (spread < 0 || spread >= totalSpreads) return;

        StopAllCoroutines();
        isFlipping = false;

        foreach (var page in pages)
            page.transform.localScale = Vector3.one;

        currentSpread = spread;
        UpdateSpreadVisibility();
    }

    public void NextPage()
    {
        if (isFlipping) return;
        if (currentSpread < totalSpreads - 1)
            StartCoroutine(AnimateFlip(currentSpread, currentSpread + 1));
    }

    public void PreviousPage()
    {
        if (isFlipping) return;
        if (currentSpread > 0)
            StartCoroutine(AnimateFlip(currentSpread, currentSpread - 1));
    }

    private void UpdateSpreadVisibility()
    {
        for (int i = 0; i < pages.Count; i++)
        {

            int spread = i / 2;
            bool show = spread == currentSpread;
            pages[i].SetActive(show);
            SetPageVisibility(i, show);
        }
    }

    private IEnumerator AnimateFlip(int from, int to)
    {
        isFlipping = true;

        bool goingForward = to > from;

        int fromLeft  = from * 2;
        int fromRight = from * 2 + 1;
        int toLeft    = to * 2;
        int toRight   = to * 2 + 1;

        if (toLeft < pages.Count)  { pages[toLeft].SetActive(true);  SetPageVisibility(toLeft, false); }
        if (toRight < pages.Count) { pages[toRight].SetActive(true); SetPageVisibility(toRight, false); }

        float half = flipDuration / 2f;
        float elapsed = 0f;

        if (goingForward)
        {
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / half);
                if (fromRight < pages.Count)
                    pages[fromRight].transform.localScale = new Vector3(1f - t, 1, 1);
                yield return null;
            }

            if (fromLeft < pages.Count)  { pages[fromLeft].SetActive(false); pages[fromLeft].transform.localScale = Vector3.one; }
            if (fromRight < pages.Count) { pages[fromRight].SetActive(false); pages[fromRight].transform.localScale = Vector3.one; }
            if (toRight < pages.Count)   SetPageVisibility(toRight, true);
            if (toLeft < pages.Count)    SetPageVisibility(toLeft, true);

            elapsed = 0f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / half);
                if (toLeft < pages.Count)
                    pages[toLeft].transform.localScale = new Vector3(t, 1, 1);
                yield return null;
            }
            if (toLeft < pages.Count) pages[toLeft].transform.localScale = Vector3.one;
        }
        else
        {
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / half);
                if (fromLeft < pages.Count)
                    pages[fromLeft].transform.localScale = new Vector3(1f - t, 1, 1);
                yield return null;
            }

            // สลับ
            if (fromLeft < pages.Count)  { pages[fromLeft].SetActive(false); pages[fromLeft].transform.localScale = Vector3.one; }
            if (fromRight < pages.Count) { pages[fromRight].SetActive(false); pages[fromRight].transform.localScale = Vector3.one; }
            if (toLeft < pages.Count)    SetPageVisibility(toLeft, true);
            if (toRight < pages.Count)   SetPageVisibility(toRight, true);

            elapsed = 0f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / half);
                if (toRight < pages.Count)
                    pages[toRight].transform.localScale = new Vector3(t, 1, 1);
                yield return null;
            }
            if (toRight < pages.Count) pages[toRight].transform.localScale = Vector3.one;
        }

        currentSpread = to;
        isFlipping = false;
    }

    private void SetPageVisibility(int index, bool isVisible)
    {
        if (index >= pages.Count) return;
        CanvasGroup cg = pages[index].GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = isVisible ? 1 : 0;
            cg.blocksRaycasts = isVisible;
            cg.interactable = isVisible;
        }
    }
}
