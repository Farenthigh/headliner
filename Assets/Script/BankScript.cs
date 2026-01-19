using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BankScript : MonoBehaviour
{
    [SerializeField] private string bankName;
    private List<Transaction> transactions = new List<Transaction>();
    private bool isContractBroken = false;
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
            if (transaction.type == Transaction.TransactionType.Deposit && transaction.isActive)
            {
                balance += transaction.amount;
            }
            else if (transaction.type == Transaction.TransactionType.Withdraw && transaction.isActive)
            {
                balance -= transaction.amount;
            }
        }
        return balance;
    }
    public void SetIsContractBroken(bool status)
    {
        isContractBroken = status;
        Debug.Log($"Bank {bankName} contract broken status set to {isContractBroken}");
    }
    public void Deposit(float amount)
    {
        Transaction newTransaction = new Transaction(Transaction.TransactionType.Deposit, amount, 0, SavingGameLogicManager.Instance.GetCurrentMonth(), SavingGameLogicManager.Instance.GetCurrentYear(), SavingGameLogicManager.Instance.GetCurrentMonth(), isContractBroken);
        SavingGameLogicManager.Instance.DeductCash(amount);
        if (isContractBroken)
        {
            Debug.Log("Deposit with contract is broken");
        }
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
        // ยกเลิกสัญญา (ถ้ามี)
        CancelContract();

        // เพิ่มเงินกลับเข้ากระเป๋าผู้เล่น (wallet)
        SavingGameLogicManager.Instance.AddCash(amount);


        //บันทึก transaction
        transactions.Add(newTransaction);
    }
    private void CancelContract()
    {
        isContractBroken = true;
        int targetMonth = SavingGameLogicManager.Instance.GetCurrentMonth();
        int targetYear = SavingGameLogicManager.Instance.GetCurrentYear();

        // เช็คว่ามี deposit ในเดือน/ปี ปัจจุบันไหม
        bool hasDepositInCurrentMonth = transactions.Any(t =>
            t.type == Transaction.TransactionType.Deposit &&
            t.isActive &&
            !t.isContractBroken &&
            !t.isInterestPaid &&
            t.transactionMonth == targetMonth &&
            t.transactionYear == targetYear
        );

        // ถ้าไม่มี → ใช้เดือน/ปี ของ deposit ล่าสุด
        if (!hasDepositInCurrentMonth)
        {
            Transaction latestDeposit = GetLatestDeposit();
            if (latestDeposit == null)
                return; // ไม่มีอะไรให้ถอน

            targetMonth = latestDeposit.transactionMonth;
            targetYear = latestDeposit.transactionYear;
        }

        foreach (var t in transactions)
        {
            if (!t.isActive ||
                t.type != Transaction.TransactionType.Deposit ||
                t.isInterestPaid ||
                t.isContractBroken)
                continue;

            if (t.transactionMonth == targetMonth &&
                t.transactionYear == targetYear)
            {
                Debug.Log($"Contract broken amount {t.amount} for deposit made in {t.transactionMonth}/{t.transactionYear}");
                t.isContractBroken = true;
            }
        }
    }
    private Transaction GetLatestDeposit()
    {
        if (transactions.Count == 0)
            return null;

        return transactions
            .Where(t => t.type == Transaction.TransactionType.Deposit)
            .Where(t => t.isActive == true)
            .OrderByDescending(t => t.transactionYear)
            .ThenByDescending(t => t.transactionMonth)
            .FirstOrDefault();
    }

}