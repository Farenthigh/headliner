using UnityEngine;
using System;
using System.Collections.Generic;

public class SavingGameLogicManager : MonoBehaviour
{
    public static SavingGameLogicManager Instance { get; private set; }
    [SerializeField] private int secondsInOneMonth = 5;
    [SerializeField] private float startingCash = 1000f;
    [SerializeField] private float cashPerMonth = 100f;
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

    private void Start()
    {
        currentMonth = 1;
        currentYear = 0;
        currentTime = 0f;
        cash = startingCash;

    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        // Debug.Log("Current Time: " + GetFullTimeInSeconds());
        if (currentTime >= secondsInOneMonth)
        {
            AdvanceMonth();
            currentTime = 0f;
        }
    }

    private void AdvanceMonth()
    {
        //TODO: clear UI notifications for new month //kf
        //call function OncloseBankPanel in SavingGameUIManager //kf
        currentMonth++;
        cash += cashPerMonth;

        List<BankScript> allBanks = GetAllBank();
        foreach (BankScript bank in allBanks)
        {
            bank.SetIsContractBroken(false);
            bank.UpdateTransactionCurrentCurrentMonth();
            bank.PayInterest();
        }



        Debug.Log($"💰 Received monthly cash: {cashPerMonth}. Current cash: {cash}");

        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }

        Debug.Log($"📅 New Month: Year {currentYear}, Month {currentMonth}");

        OnNewMonth?.Invoke(currentYear, currentMonth);

        SavingGameUIManager.Instance.OnCloseBankPanel();
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
        if (currentGold < 0) currentGold = 0;
        Debug.Log($"Player Gold Updated: {currentGold}");
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
            Debug.LogWarning("Not enough cash!");
            return false;
        }
    }
    public float GetFullTimeInSeconds()
    {
        return (currentYear * 12 + currentMonth - 1) * secondsInOneMonth + currentTime;
    }
    public List<BankScript> GetAllBank()
    {
        List<BankScript> allBanks = new List<BankScript>(FindObjectsByType<BankScript>(FindObjectsSortMode.None));
        return allBanks;
    }
}
