namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Date { get; set; }

        public Expense(int id, int categoryId, string description, double amount, string date)
        {
            Id = id;
            CategoryId = categoryId;
            Description = description;
            Amount = amount;
            Date = date;
        }
    }
}
