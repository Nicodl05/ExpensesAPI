namespace ExpensesAPI.Exceptions
{
    public class DuplicateExpenseException : Exception
    {
        public DuplicateExpenseException(DateTime date, decimal amount)
            : base($"An expense with date {date.ToShortDateString()} and amount {amount} has already been declared.") { }
    }
}
