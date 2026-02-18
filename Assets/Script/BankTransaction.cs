public class Transaction
{
    public enum TransactionType { Deposit, Withdraw }
    public TransactionType type;
    public float amount;
    public int transactionMonth = 0;
    public int transactionYear = 0;
    public int currentMonth = 0;
    public bool isActive = true;
    public bool isInterestPaid = false;
    public bool isContractBroken = false;

    public Transaction(TransactionType type, float amount, int transactionMonth, int transactionYear, bool isContractBroken = false)
    {
        this.type = type;
        this.amount = amount;
        this.transactionMonth = transactionMonth;
        this.transactionYear = transactionYear;
        this.isContractBroken = isContractBroken;
    }
}