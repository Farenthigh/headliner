using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Interest
{
    public float interestRate; // e.g., 0.05 for 5%
    public int durationInMonths; // e.g., 12 for 1 year
}
public enum InterestPayType
{
    Roundly,
    Target
}

public class BankScript : MonoBehaviour
{
    [SerializeField] private string bankName;
    [SerializeField] private List<Interest> interestOptions;
    [SerializeField] private InterestPayType interestPayType;
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
        Transaction transaction = new Transaction(
            Transaction.TransactionType.Deposit,
            amount,
            SavingGameLogicManager.Instance.GetCurrentMonth(),
            SavingGameLogicManager.Instance.GetCurrentYear(),
            isContractBroken
        );
        SavingGameLogicManager.Instance.DeductCash(amount);
        if (isContractBroken)
        {
            Debug.Log("Deposit with contract is broken");
        }
        transactions.Add(transaction);
    }
    public void Withdraw(float amount)
    {
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
            SavingGameLogicManager.Instance.GetCurrentMonth(),
            SavingGameLogicManager.Instance.GetCurrentYear(),
            isContractBroken
        );
        //จ่ายดอกเบี้ยถ้าหากตรงตามเงื่อนไข
        if (interestPayType == InterestPayType.Target) PayInterest();
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
    public void PayInterest()
    {
        //TODO:: Implement interest payment functionality. with target monthly that find most suitable interest option when wtthdraw
        List<Transaction> newTransactions = new List<Transaction>();
        Interest selectedInterest = null;
        foreach (var deposit in transactions)
        {
            if (!deposit.isActive ||
                deposit.type != Transaction.TransactionType.Deposit ||
                deposit.isInterestPaid ||
                deposit.isContractBroken)
                continue;

            if (interestOptions.Count == 0)
                continue;

            else if (interestOptions.Count > 1)
            {
                foreach (var interest in interestOptions)
                {
                    if (deposit.currentMonth >= interest.durationInMonths)
                    {
                        selectedInterest = interest;
                    }
                }
            }
            else if (interestOptions.Count == 1)
            {
                selectedInterest = interestOptions[0];
            }
            if (deposit.currentMonth == selectedInterest.durationInMonths)
            {
                float interestAmount = deposit.amount * selectedInterest.interestRate;

                // ดอกเบี้ย
                newTransactions.Add(new Transaction(
                    Transaction.TransactionType.Deposit,
                    interestAmount,
                    SavingGameLogicManager.Instance.GetCurrentMonth(),
                    SavingGameLogicManager.Instance.GetCurrentYear()
                ));

                // ปิดสัญญาเก่า
                deposit.isActive = false;
                deposit.isInterestPaid = true;

                // ฝากต่อ
                newTransactions.Add(new Transaction(
                    Transaction.TransactionType.Deposit,
                    deposit.amount + interestAmount, // ทบต้น
                    SavingGameLogicManager.Instance.GetCurrentMonth(),
                    SavingGameLogicManager.Instance.GetCurrentYear()
                ));

                Debug.Log($"Paid interest of {interestAmount} for deposit {deposit.transactionMonth}/{deposit.transactionYear}");
                break;
            }
        }
        transactions.AddRange(newTransactions);
    }
    public void UpdateTransactionCurrentCurrentMonth()
    {
        foreach (var transaction in transactions)
        {
            if (transaction.isActive && transaction.type == Transaction.TransactionType.Deposit && !transaction.isContractBroken)
                transaction.currentMonth += 1;
        }
    }
    //TODO:: Create an Condition and withdraw penalty function
}