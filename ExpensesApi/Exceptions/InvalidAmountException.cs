namespace ExpensesAPI.Exceptions
{
    public class InvalidAmountException : Exception
    {
        public InvalidAmountException(decimal amount)
            : base($"Amount must be greater than zero. Provided amount: {amount}.")
        {
        }
    }
}
