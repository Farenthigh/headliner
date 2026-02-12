using System.Collections.Generic;
using UnityEngine;

public class Transaction
{
    public enum TransactionType { Deposit, Withdraw }
    public TransactionType type;
    public float amount;
    public int roundMonth;
    public int transactionMonth;
    public int transactionYear;
    public int currentMonth;
    public bool isActive = true;
    public bool isInterestPaid = false;
    public bool isContractBroken = false;

    public Transaction(TransactionType transactionType, float transactionAmount, int transactionRoundMonth, int transactionMonth, int transactionYear, int currentMonth)
    {
        type = transactionType;
        amount = transactionAmount;
        this.roundMonth = transactionRoundMonth;
        this.transactionMonth = transactionMonth;
        this.transactionYear = transactionYear;
        this.currentMonth = currentMonth;
    }
}
public class BankScript : MonoBehaviour
{
    [SerializeField] private string bankName;
    private List<Transaction> transactions = new List<Transaction>();
    private void OnMouseDown()
    {   
        if (SavingGameUIManager.Instance.GetBankPanel() != true)
            SavingGameUIManager.Instance.OnOpenBankPanel(this);
    }
    public float GetBalance()
    {
        float balance = 0f;
        foreach (Transaction transaction in transactions)
        {
            if (transaction.type == Transaction.TransactionType.Deposit)
            {
                balance += transaction.amount;
            }
            else if (transaction.type == Transaction.TransactionType.Withdraw)
            {
                balance -= transaction.amount;
            }
        }
        return balance;
    }
    public void Deposit(float amount)
    {
        Transaction newTransaction = new Transaction(Transaction.TransactionType.Deposit, amount, 0, SavingGameLogicManager.Instance.GetCurrentMonth(), SavingGameLogicManager.Instance.GetCurrentYear(), SavingGameLogicManager.Instance.GetCurrentMonth());
        SavingGameLogicManager.Instance.DeductCash(amount);
        transactions.Add(newTransaction);
    }
    public void Withdraw(float amount)
    {
        //TODO: Implement withdraw functionality //eve
        //wallet += withdraw
        // ตรวจสอบจำนวนเงิน

        if (amount <= 0f)
            return;

        // ตรวจสอบว่าเงินในธนาคารพอไหม
        float balance = GetBalance();

        if (amount > balance)
        {
            Debug.LogWarning("Not enough balance in bank");
            return;
        }

        // สร้าง Transaction แบบถอนเงิน
        Transaction newTransaction = new Transaction(
            Transaction.TransactionType.Withdraw,
            amount,
            0,
            SavingGameLogicManager.Instance.GetCurrentMonth(),
            SavingGameLogicManager.Instance.GetCurrentYear(),
            SavingGameLogicManager.Instance.GetCurrentMonth()
        );

        // เพิ่มเงินกลับเข้ากระเป๋าผู้เล่น (wallet)
        SavingGameLogicManager.Instance.AddCash(amount);
        

        //บันทึก transaction
        transactions.Add(newTransaction);
        
    
    }
}