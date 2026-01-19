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

    public Transaction(TransactionType type, float amount, int transactionRoundMonth, int transactionMonth, int transactionYear, int currentMonth, bool isContractBroken = false)
    {
        this.type = type;
        this.amount = amount;
        this.roundMonth = transactionRoundMonth;
        this.transactionMonth = transactionMonth;
        this.transactionYear = transactionYear;
        this.currentMonth = currentMonth;
        this.isContractBroken = isContractBroken;
    }
}