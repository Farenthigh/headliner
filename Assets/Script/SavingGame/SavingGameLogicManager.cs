using UnityEngine;
using System;
using System.Collections.Generic;

public class SavingGameLogicManager : MonoBehaviour
{
    public static SavingGameLogicManager Instance { get; private set; }
    [SerializeField] private int secondsInOneMonth = 5;
    [SerializeField] private float startingCash = 1000f;
    [SerializeField] private float cashPerMonth = 1000f;
    [SerializeField] private int goalMonth = 12;
    [SerializeField] public BankScript[] banks;
    public static event Action<int, int> OnNewMonth;
    // year, month

    private bool isPaused = false;
    private int currentMonth = 1;
    private int currentYear = 0;
    private float currentTime;
    private float cash;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentMonth = 1;
        currentYear = 0;
        currentTime = 0f;
        cash = startingCash;
        // SavingGameUIManager.Instance.UpdateGoalBar();
    }

    private void Update()
    {
        if (!isPaused) currentTime += Time.deltaTime;

        SavingGameUIManager.Instance.UpdateRoundTime(currentTime, secondsInOneMonth); //ใช้ทำUpdateRoundTime
        SavingGameUIManager.Instance.UpdateRoundMonth();

        // Debug.Log("Current Time: " + GetFullTimeInSeconds());
        if (currentTime >= secondsInOneMonth)
        {
            currentTime -= secondsInOneMonth;

            AdvanceMonth();
            // EventManager.Instance.RandomEvent();
        }
    }

    public void AdvanceMonth()
    {   
        

        currentMonth++;
        if (SavingGameUIManager.Instance != null)
        {
            SavingGameUIManager.Instance.SpawnFloatingRound(1);
        }
        if (currentMonth > SavingGoalManager.Instance.GetGoalMonth())
        {
            Debug.Log("Goal Month Reached! Checking goals...");
            SavingGoalManager.Instance.CheckGoals(GetAllAssets());
            SetIsPaused(true); // หยุดเกมเมื่อถึงเดือนเป้าหมาย
            return;
        }
        cash += cashPerMonth;
        SetIsPaused(true); // หยุดเกมชั่วคราวระหว่างการคำนวณและแสดงผล
        SavingGameUIManager.Instance.UpdateRoundMonth();
        Debug.Log($"💰 Received monthly cash: {cashPerMonth}. Current cash: {cash}");

        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }

        Debug.Log($"📅 New Month: Year {currentYear}, Month {currentMonth}");

        OnNewMonth?.Invoke(currentYear, currentMonth);

        foreach (var bank in banks)
        {
            if (bank != null)
            {
                bank.UpdateTransactionCurrentMonth();
                bank.PayInterest();
            }
        }
        HomePanel.Instance.CloseHomePanel();
        BankPanelUI.Instance.OnCloseBankPanel();
        EventManager.Instance.ResetEventTrigger();
        EventManager.Instance.RandomEvent();

    }
    public void Addtime(int time)
    {
        if (SavingGameUIManager.Instance != null && time != 0)
        {
            SavingGameUIManager.Instance.SpawnFloatingTime(time);
        }
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
        Debug.Log($"Player Gold Updated: {cash}");

        if (SavingGameUIManager.Instance != null && amount != 0)
        {
            SavingGameUIManager.Instance.SpawnFloatingMoney(amount);
        }
    }
    public bool DeductCash(float amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            if (SavingGameUIManager.Instance != null && amount != 0)
            {
                SavingGameUIManager.Instance.SpawnFloatingMoney(-amount);
            }
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough cash!");
            return false;
        }
    }
    public float GetFullTimeInSeconds()
    {
        return (currentYear * 12 + currentMonth - 1) * secondsInOneMonth + currentTime;
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
    public void SetIsPaused(bool paused)
    {
        isPaused = paused;
    }
}
