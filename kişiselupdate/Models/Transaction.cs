namespace ExpenseTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public int ExpenseId { get; set; }
        public string Date { get; set; }

        public Transaction(int id, string transactionType, int expenseId, string date)
        {
            Id = id;
            TransactionType = transactionType;
            ExpenseId = expenseId;
            Date = date;
        }
    }
}
