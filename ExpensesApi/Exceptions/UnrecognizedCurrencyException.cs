using ExpensesAPI.Domain;

namespace ExpensesAPI.Exceptions
{
    public class UnrecognizedCurrencyException : Exception
    {
        public UnrecognizedCurrencyException(Currency expenseCurrency)
            : base($"The expense currency ({expenseCurrency}) isn't recognized.") { }
    }
}
