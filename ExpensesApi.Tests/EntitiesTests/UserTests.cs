using ExpensesAPI.Domain;
using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Tests
{
    public class UserTests
    {
        [Fact]
        public void UserShouldBeCreatedWithCorrectValuesTest()
        {
            var firstName = "Tony";
            var lastName = "Stark";
            var currency = Currency.USD;

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Currency = currency
            };

            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(currency, user.Currency);
        }

        [Fact]
        public void UserShouldReturnCorrectFullNameTest()
        {

            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };

            var fullName = user.FullName;
            Assert.Equal("Tony Stark", fullName);
        }

        [Fact]
        public void UsersWithDifferentPropertiesShouldNotBeEqual()
        {

            var user1 = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.EUR };
            var user2 = new User { FirstName = "Bruce", LastName = "Wayne", Currency = Currency.USD };
            Assert.NotEqual(user1.FirstName, user2.FirstName);
            Assert.NotEqual(user1.LastName, user2.LastName);
            Assert.NotEqual(user1.Currency, user2.Currency);
        }

        [Fact]
        public void CheckCurrencyShouldReturnTrueWhenCurrenciesMatch()
        {

            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };

            var result = user.CheckCurrency(Currency.USD);
            Assert.True(result);
        }

        [Fact]
        public void CheckCurrencyShouldReturnFalseWhenCurrenciesDoNotMatch()
        {

            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };

            var result = user.CheckCurrency(Currency.EUR);
            Assert.False(result);
        }

        [Fact]
        public void CheckCurrencyShouldThrowUnrecognizedCurrencyExceptionWhenInvalidCurrencyProvided()
        {
            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };

            var ex = Assert.Throws<UnrecognizedCurrencyException>(() => user.CheckCurrency((Currency)(-1)));
            Assert.Equal("The expense currency (-1) isn't recognized.", ex.Message);
        }
    }
}
