using ExpensesAPI.Domain;
using ExpensesAPI.Exceptions;
using ExpensesAPI.Repositories;
using NSubstitute;


namespace ExpensesAPI.Services.Tests
{
    public class ExpenseServiceTests
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceTests()
        {
            _expenseRepository = Substitute.For<IExpenseRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _expenseService = new ExpenseService(_expenseRepository, _userRepository);
        }

        //******************************
        //******************************
        // Add Expense
        //******************************
        //******************************

        [Fact]
        public void AddExpenseShouldAddExpenseWhenValidExpense()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);

            _expenseService.AddExpense(expense);
            _expenseRepository.Received(1).AddExpense(expense);
        }

        [Fact]
        public void AddExpenseShouldThrowDuplicateExpenseExceptionWhenExpenseIsDuplicate()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var duplicateExpense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);

            _expenseRepository.CheckDuplicate(duplicateExpense).Returns(true);
            Assert.Throws<DuplicateExpenseException>(() => _expenseService.AddExpense(duplicateExpense));
        }


        [Fact]
        public void AddExpenseShouldThrowInvalidExpenseDateException_WhenDateIsMoreThanThreeMonthsOld()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            Assert.Throws<InvalidExpenseDateException>(() => new Expense(DateTime.Now.AddMonths(-4), ExpenseType.Restaurant, 100, Currency.USD, "Old expense", user));
        }

        [Fact]
        public void AddExpenseShouldAddUserWhenUserDoesNotExist()
        {
            var user = new User { FirstName = "Peter", LastName = "Parker", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Misc, 50, Currency.USD, "Expense for new user", user);

            _userRepository.GetUserByDetails(user.FirstName, user.LastName, user.Currency).Returns((User)null);
            _expenseService.AddExpense(expense);

            _userRepository.Received(1).AddUser(Arg.Is<User>(u => u.FirstName == "Peter" && u.LastName == "Parker" && u.Currency == Currency.USD));
        }

        [Fact]
        public void AddExpenseShouldThrowMissingCommentExceptionWhenCommentIsEmpty()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            Assert.Throws<MissingCommentException>(() => new Expense(DateTime.Now.AddDays(-1), ExpenseType.Misc, 50, Currency.USD, "", user));
        }


        [Fact]
        public void AddExpenseShouldThrowInvalidCurrencyExceptionWhenCurrencyDoesNotMatchUserCurrency()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            Assert.Throws<InvalidCurrencyException>(() => new Expense(DateTime.Now.AddDays(-1), ExpenseType.Misc, 100, Currency.EUR, "Wrong currency", user));
        }


        //******************************
        //******************************
        // Get Expenses
        //******************************
        //******************************

        [Theory]
        [InlineData("asc", 50, 200)]
        [InlineData("desc", 200, 50)]
        public void GetExpensesByUserShouldReturnExpensesSortedByAmount(string sortOrder, decimal expectedFirstAmount, decimal expectedLastAmount)
        {
            var userId = Guid.NewGuid();
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            _userRepository.GetUserById(userId).Returns(user);

            var expense1 = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 50, Currency.USD, "Lunch", user);
            var expense2 = new Expense(DateTime.Now.AddDays(-2), ExpenseType.Hotel, 200, Currency.USD, "Hotel stay", user);
            _expenseRepository.GetExpensesByUser(userId).Returns(new List<Expense> { expense1, expense2 });
            var result = _expenseService.GetExpensesByUser(userId, "amount", sortOrder).ToList();


            Assert.Equal(expectedFirstAmount, result.First().Amount);
            Assert.Equal(expectedLastAmount, result.Last().Amount);
            Assert.Equal("Tony Stark", result.First().UserFullName);
            Assert.Equal("Tony Stark", result.Last().UserFullName);
        }

        [Theory]
        [InlineData("asc", -2, -1)]
        [InlineData("desc", -1, -2)]
        public void GetExpensesByUserShouldReturnExpensesSortedByDate(string sortOrder, int expectedFirstDaysOffset, int expectedLastDaysOffset)
        {
            var userId = Guid.NewGuid();
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            _userRepository.GetUserById(userId).Returns(user);

            var expense1 = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Lunch", user);
            var expense2 = new Expense(DateTime.Now.AddDays(-2), ExpenseType.Hotel, 150, Currency.USD, "Hotel stay", user);
            _expenseRepository.GetExpensesByUser(userId).Returns(new List<Expense> { expense1, expense2 });
            var result = _expenseService.GetExpensesByUser(userId, "date", sortOrder).ToList();

            Assert.Equal(DateTime.Now.AddDays(expectedFirstDaysOffset).Date, result.First().Date.Date);
            Assert.Equal(DateTime.Now.AddDays(expectedLastDaysOffset).Date, result.Last().Date.Date);
            Assert.Equal("Tony Stark", result.First().UserFullName);
            Assert.Equal("Tony Stark", result.Last().UserFullName);
        }

        [Fact]
        public void GetExpensesByUserShouldReturnExpensesWhenExpensesExist()
        {
            var userId = Guid.NewGuid();
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };

            _userRepository.GetUserById(userId).Returns(user);
            var expenses = new List<Expense>
    {
        new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user),
        new Expense(DateTime.Now.AddDays(-2), ExpenseType.Hotel, 200, Currency.USD, "Hotel stay", user)
    };
            _expenseRepository.GetExpensesByUser(userId).Returns(expenses);
            var result = _expenseService.GetExpensesByUser(userId, "date").ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Tony Stark", result[0].UserFullName);
            Assert.Equal(200, result[0].Amount);
            Assert.Equal("Hotel stay", result[0].Comment);
            Assert.Equal(100, result[1].Amount);
            Assert.Equal("Dinner", result[1].Comment);
        }

        [Fact]
        public void GetExpensesByUserShouldReturnExpensesSortedByDateDescending()
        {
            var userId = Guid.NewGuid();
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            _userRepository.GetUserById(userId).Returns(user);

            var expense1 = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 50, Currency.USD, "Lunch", user);
            var expense2 = new Expense(DateTime.Now.AddDays(-2), ExpenseType.Hotel, 200, Currency.USD, "Hotel stay", user);
            _expenseRepository.GetExpensesByUser(userId).Returns(new List<Expense> { expense1, expense2 });

            var result = _expenseService.GetExpensesByUser(userId, "date", "desc").ToList();
            Assert.Equal(expense1.Date, result.First().Date);
            Assert.Equal(expense2.Date, result.Last().Date);
        }


    }
}
