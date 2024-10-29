using ExpensesAPI.Domain;
using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Tests
{
    public class ExpenseTests
    {
        [Fact]
        public void ExpenseShouldBeCreatedWithCorrectValues()
        {
            var date = DateTime.Now.AddDays(-1);
            var type = ExpenseType.Restaurant;
            var amount = 100m;
            var currency = Currency.USD;
            var comment = "Dinner at restaurant";
            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };

            var expense = new Expense(date, type, amount, currency, comment, user);
            Assert.Equal(date, expense.Date);
            Assert.Equal(type, expense.Type);
            Assert.Equal(amount, expense.Amount);
            Assert.Equal(currency, expense.Currency);
            Assert.Equal(comment, expense.Comment);
            Assert.Equal(user, expense.User);
        }

        [Fact]
        public void ExpenseShouldThrowInvalidExpenseDateExceptionWhenDateIsInFuture()
        {
            var futureDate = DateTime.Now.AddDays(1);
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };

            var ex = Assert.Throws<InvalidExpenseDateException>(() =>
                new Expense(futureDate, ExpenseType.Restaurant, 100m, Currency.USD, "Dinner", user)
            );
            Assert.Equal("The expense date " + futureDate.ToShortDateString() + " cannot be in the future.", ex.Message);
        }

        [Fact]
        public void ExpenseShouldThrowMissingCommentExceptionWhenCommentIsMissing()
        {

            var date = DateTime.Now.AddDays(-1);
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };


            var ex = Assert.Throws<MissingCommentException>(() =>
                new Expense(date, ExpenseType.Hotel, 200m, Currency.USD, "", user)
            );

            Assert.Equal("Comment is mandatory.", ex.Message);
        }

        [Fact]
        public void ExpenseShouldThrowInvalidCurrencyExceptionWhenCurrencyDoesNotMatchUserCurrency()
        {

            var date = DateTime.Now.AddDays(-1);
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };


            var ex = Assert.Throws<InvalidCurrencyException>(() =>
                new Expense(date, ExpenseType.Misc, 150m, Currency.EUR, "Misc expense", user)
            );

            Assert.Equal("The expense currency (EUR) does not match the user's currency (USD).", ex.Message);
        }

        [Fact]
        public void ExpenseShouldThrowInvalidAmountExceptionWhenAmountIsZeroOrNegative()
        {
            var date = DateTime.Now.AddDays(-1);
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };

            var exZero = Assert.Throws<InvalidAmountException>(() =>
                new Expense(date, ExpenseType.Restaurant, 0m, Currency.USD, "Dinner", user)
            );
            Assert.Equal("Amount must be greater than zero. Provided amount: 0.", exZero.Message);

            var exNegative = Assert.Throws<InvalidAmountException>(() =>
                new Expense(date, ExpenseType.Restaurant, -50m, Currency.USD, "Dinner", user)
            );
            Assert.Equal("Amount must be greater than zero. Provided amount: -50.", exNegative.Message);
        }
    }
}
