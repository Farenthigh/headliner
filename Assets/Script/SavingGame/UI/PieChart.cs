
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField] private GameObject slicePrefab;
    [SerializeField] private Transform slicesParent;

    public static PieChart Instance { get; private set; }
    private List<float> Values = new List<float>();
    private List<Color> Colors = new List<Color>()
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0.5f, 0f, 1f), // Purple
        new Color(0f, 1f, 0.5f), // Teal
        new Color(1f, 1f, 0.5f)  // Light Yellow
    };
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        GetAllAssets();
    }

    public void CreatePieChart(List<float> values, List<Color> colors)
    {
        // ลบ slices เก่า
        foreach (Transform child in slicesParent)
        {
            Destroy(child.gameObject);
        }

        float total = 0f;
        foreach (float value in values)
        {
            total += value;
        }

        float zRotation = 0f;
        for (int i = 0; i < values.Count; i++)
        {
            float sliceAmount = values[i] / total;
            GameObject newSlice = Instantiate(slicePrefab, slicesParent);
            newSlice.GetComponent<Image>().color = colors[i];
            newSlice.GetComponent<Image>().fillAmount = sliceAmount;
            newSlice.transform.rotation = Quaternion.Euler(0f, 0f, -zRotation);
            zRotation += sliceAmount * 360f;
            Debug.Log($"Slice {i}: {sliceAmount * 100f:F2}%");
        }
    }
    public void GetAllAssets()
    {
        Values.Clear();
        foreach (BankScript bank in SavingGameLogicManager.Instance.banks)
        {
            Values.Add(bank.GetBalance());
        }
        Values.Add(SavingGameLogicManager.Instance.GetCurrentCash());
        CreatePieChart(Values, Colors);
    }
}
