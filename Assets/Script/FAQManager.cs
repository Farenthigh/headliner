using UnityEngine;

public class FAQManager : MonoBehaviour
{
    public static FAQManager instance;

    FAQItem[] items;

    void Awake()
    {
        instance = this;
        items = GetComponentsInChildren<FAQItem>();
    }

    public void CloseAll()
    {
        foreach (FAQItem item in items)
        {
            item.Close();
        }
    }
}