using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SavingGameLogicManager : MonoBehaviour
{
    public static SavingGameLogicManager Instance { get; private set; }
    [SerializeField] private int secondsInOneMonth = 5;
    [SerializeField] private float startingCash = 1000f;
    [SerializeField] private float cashPerMonth = 100f;
    [SerializeField] private float goalAmount = 10000f;
    [SerializeField] private int goalMonth = 12;
    [SerializeField] private BankScript[] banks;
    [SerializeField] private int star = 1;
    public static event Action<int, int> OnNewMonth;
    // year, month

    private int currentMonth = 1;
    private int currentYear = 0;
    private float currentTime;
    private float cash;
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

    private IEnumerator Start()
    {
        yield return null;
        currentMonth = 1;
        currentYear = 0;
        currentTime = 0f;
        cash = startingCash;
        SavingGameUIManager.Instance.UpdateGoalBar();
        SavingGameUIManager.Instance.UpdateVictory();
        SavingGameUIManager.Instance.UpdateDefeat();
        // SavingGameUIManager.Instance.UpdateGoalBar(); 
        if (SavingGameUIManager.Instance != null)
        {
            SavingGameUIManager.Instance.UpdateGoalBar();
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        // Debug.Log("Current Time: " + GetFullTimeInSeconds());

        SavingGameUIManager.Instance.UpdateRoundTime(currentTime, secondsInOneMonth); //ใช้ทำUpdateRoundTimeg
        SavingGameUIManager.Instance.UpdateRoundMonth();
        SavingGameUIManager.Instance.UpdateVictory(); // ให้แสดงนห้า ui victory 
        SavingGameUIManager.Instance.UpdateDefeat();
        
        if (currentTime >= secondsInOneMonth)
        {
            currentTime -= secondsInOneMonth;

            AdvanceMonth();
            EventManager.Instance.RandomEvent();
        }
    }

    private void AdvanceMonth()
    {
        currentMonth++;
        cash += cashPerMonth;

        if(EventManager.Instance != null)
        {
            EventManager.Instance.RandomEvent();
            Time.timeScale = 0f; // Pause the game when event is triggered
        }
        
        SavingGameUIManager.Instance.UpdateGoalBar();
        SavingGameUIManager.Instance.UpdateRoundMonth();

        // Debug.Log($"💰 Received monthly cash: {cashPerMonth}. Current cash: {cash}");
       
        Debug.Log($"💰 Received monthly cash: {cashPerMonth}. Current cash: {cash}");

        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }

        // Debug.Log($"📅 New Month: Year {currentYear}, Month {currentMonth}");

        OnNewMonth?.Invoke(currentYear, currentMonth);

        SavingGameUIManager.Instance.OnCloseBankPanel();
        SavingGameUIManager.Instance.UpdateVictory();
        SavingGameUIManager.Instance.UpdateDefeat();
        EventManager.Instance.ResetEventTrigger();

    }
    public int GetCurrentMonth()
    {
        return currentMonth;
    }
    public int GetCurrentYear()
    {
        return currentYear;
    }
    public float GetCurrentCash()
    {
        return cash;
    }
    public void AddCash(float amount)
    {
        cash += amount;
        if (cash < 0) cash = 0;
        // Debug.Log($"Player Gold Updated: {cash}");
    }
    public bool DeductCash(float amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            return true;
        }
        else
        {
            // Debug.LogWarning("Not enough cash!");
            return false;
        }
    }
    public float GetFullTimeInSeconds()
    {
        return (currentYear * 12 + currentMonth - 1) * secondsInOneMonth + currentTime;
    }

    public void ModifyTime(float seconds)
    {
        currentTime += seconds;
        if (currentTime >= secondsInOneMonth)
        {
            AdvanceMonth();
            currentTime = 0f;
        }
        else if(currentTime < 0 )
        {
            currentTime = 0f; // Prevent going to previous month for simplicity
        }

        // Debug.Log($"[Event] Time Modified: {seconds}s. Current Time in month: {currentTime}s");
    }
    public float GetGoalAmount()
    {
    return goalAmount;
    }

public float GetAllAssets()
{
    float total = cash; // เงินสด

    foreach (BankScript bank in banks)
    {
        if (bank != null)
            total += bank.GetBalance();
    }

    return total;
}

    public int GetGoalMonth()
    {
    return goalMonth;
    }
    
public int GetStar()
{
    return star;
}

}
