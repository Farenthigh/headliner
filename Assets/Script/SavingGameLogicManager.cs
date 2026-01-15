using UnityEngine;
using System;

public class SavingGameLogicManager : MonoBehaviour
{
    [SerializeField] private int secondsInOneMonth = 5;

    public static event Action<int, int> OnNewMonth;
    // year, month

    private int currentMonth;
    private int currentYear;
    private float currentTime;

    private void Start()
    {
        currentMonth = 1;
        currentYear = 0;
        currentTime = 0f;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= secondsInOneMonth)
        {
            currentTime = 0f;
            AdvanceMonth();
        }
    }

    private void AdvanceMonth()
    {
        currentMonth++;

        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }

        Debug.Log($"📅 New Month: Year {currentYear}, Month {currentMonth}");

        OnNewMonth?.Invoke(currentYear, currentMonth);
    }
    public int GetCurrentMonth()
    {
        return currentMonth;
    }
    public int GetCurrentYear()
    {
        return currentYear;
    }
}
