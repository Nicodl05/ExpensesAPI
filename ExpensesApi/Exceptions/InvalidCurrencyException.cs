using ExpensesAPI.Domain;

namespace ExpensesAPI.Exceptions
{
    public class InvalidCurrencyException : Exception
    {
        public InvalidCurrencyException(Currency expenseCurrency, Currency userCurrency)
            : base($"The expense currency ({expenseCurrency}) does not match the user's currency ({userCurrency}).") { }
    }
}
