using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Domain
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required Currency Currency { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public User()
        {

        }
        public bool CheckCurrency(Currency currency)
        {
            if (!Enum.IsDefined(typeof(Currency), currency))
            {
                throw new UnrecognizedCurrencyException(currency);
            }

            return currency == this.Currency;
        }
    }
}
