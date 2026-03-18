using UnityEngine;

public class FAQItem : MonoBehaviour
{
    public GameObject answerPanel;
    public Transform arrow;

    public void Toggle()
    {
        bool isOpen = answerPanel.activeSelf;

        FAQManager.instance.CloseAll();

        if (!isOpen)
        {
            answerPanel.SetActive(true);

            if (arrow != null)
                arrow.rotation = Quaternion.Euler(0,0,180);
        }
    }

    public void Close()
    {
        answerPanel.SetActive(false);

        if (arrow != null)
            arrow.rotation = Quaternion.Euler(0,0,0);
    }
}