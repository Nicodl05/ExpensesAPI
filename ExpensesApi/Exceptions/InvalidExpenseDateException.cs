namespace ExpensesAPI.Exceptions
{
    public class InvalidExpenseDateException : Exception
    {
        public InvalidExpenseDateException(DateTime date, string message = null)
            : base(message ?? $"The expense date {date.ToShortDateString()} cannot be in the future.") { }
    }
}
