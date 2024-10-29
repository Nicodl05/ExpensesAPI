using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Domain
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ExpenseType Type { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string Comment { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Expense() { }
        public Expense(DateTime date, ExpenseType type, decimal amount, Currency currency, string comment, User user)
        {
            ValidateExpense(date, amount, currency, comment, user);
            Date = date;
            Type = type;
            Amount = amount;
            Currency = currency;
            Comment = comment;
            User = user;
        }
        private static void ValidateExpense(DateTime date, decimal amount, Currency currency, string comment, User user)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException(amount);
            }
            if (date > DateTime.Now)
            {
                throw new InvalidExpenseDateException(date);
            }
            if (date < DateTime.Now.AddMonths(-3))
            {
                throw new InvalidExpenseDateException(date, "The expense cannot be dated more than 3 months ago.");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new MissingCommentException();
            }
            if (currency != user.Currency)
            {
                throw new InvalidCurrencyException(currency, user.Currency);
            }
        }

    }
}
