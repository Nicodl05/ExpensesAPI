using ExpensesAPI.Controllers;
using ExpensesAPI.Domain;
using ExpensesAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ExpensesAPI.Tests.Controllers
{
    public class ExpensesControllerTests
    {
        private readonly IExpenseService _expenseService;
        private readonly ExpensesController _controller;

        public ExpensesControllerTests()
        {
            _expenseService = Substitute.For<IExpenseService>();
            _controller = new ExpensesController(_expenseService);
        }

        //*************************************
        // ************************************
        // Create Expense Tests
        //*************************************
        //*************************************
        [Fact]
        public void CreateExpenseShouldReturnOkWhenExpenseIsCreatedSuccessfully()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);
            _expenseService.When(x => x.AddExpense(expense)).Do(x => { });

            var result = _controller.CreateExpense(expense);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var message = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Expense created successfully.", message);
        }

        [Fact]
        public void CreateExpenseShouldThrowInvalidExpenseDateExceptionWhenExpenseDateIsInFuture()
        {

            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var futureDate = DateTime.Now.AddDays(1);
            var ex = Assert.Throws<InvalidExpenseDateException>(() =>
                new Expense(futureDate, ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user)
            );

            Assert.Equal($"The expense date {futureDate.ToShortDateString()} cannot be in the future.", ex.Message);
        }

        [Fact]
        public void CreateExpenseShouldReturnConflictWhenExpenseIsDuplicate()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);

            _expenseService.When(x => x.AddExpense(expense)).Do(x => { throw new DuplicateExpenseException(expense.Date, expense.Amount); });

            var result = _controller.CreateExpense(expense);
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal($"An expense with date {expense.Date.ToShortDateString()} and amount {expense.Amount} has already been declared.", conflictResult.Value);
        }

        [Fact]
        public void CreateExpenseShouldThrowInvalidAmountExceptionWhenAmountIsZeroOrNegative()
        {

            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var invalidAmount = 0;
            var ex = Assert.Throws<InvalidAmountException>(() =>
                new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, invalidAmount, Currency.USD, "Dinner", user)
            );

            Assert.Equal($"Amount must be greater than zero. Provided amount: {invalidAmount}.", ex.Message);
        }

        [Fact]
        public void CreateExpenseShouldThrowInvalidCurrencyExceptionWhenCurrencyDoesNotMatchUserCurrency()
        {

            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var ex = Assert.Throws<InvalidCurrencyException>(() =>
                new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.EUR, "Dinner", user)
            );
            Assert.Equal("The expense currency (EUR) does not match the user's currency (USD).", ex.Message);
        }

        [Fact]
        public void CreateExpenseShouldReturnBadRequestWhenCommentIsEmpty()
        {

            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };

            Assert.Throws<MissingCommentException>(() => new Expense(DateTime.Now.AddDays(-1), ExpenseType.Misc, 100, Currency.USD, "", user));

        }

        [Fact]
        public void CreateExpenseShouldReturnNotFoundWhenUserDoesNotExist()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);

            _expenseService.When(x => x.AddExpense(expense))
                           .Do(x => { throw new UserNotFoundException(user.Id); });

            var result = _controller.CreateExpense(expense);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"User with ID {user.Id} not found.", notFoundResult.Value);
        }

        [Fact]
        public void CreateExpenseShouldReturnOkWhenAllParametersAreValid()
        {
            var user = new User { FirstName = "Natasha", LastName = "Romanova", Currency = Currency.RUB };
            var expense = new Expense(DateTime.Now.AddDays(-2), ExpenseType.Restaurant, 150, Currency.RUB, "Lunch", user);

            _expenseService.When(x => x.AddExpense(expense)).Do(x => { });

            var result = _controller.CreateExpense(expense);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Expense created successfully.", okResult.Value);
        }

        [Fact]
        public void CreateExpenseShouldReturnInternalServerErrorWhenUnexpectedExceptionOccurs()
        {

            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Unexpected error test", user);

            _expenseService.When(x => x.AddExpense(Arg.Any<Expense>()))
                           .Do(x => throw new Exception("Unexpected error"));

            var result = _controller.CreateExpense(expense);
            var serverErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Unexpected error", serverErrorResult.Value);
        }



        //*************************************
        // ************************************
        // Get Expense Tests
        //*************************************
        //*************************************

        [Fact]
        public void GetExpensesShouldReturnOkWhenExpensesAreReturned()
        {
            Guid userId = Guid.NewGuid();
            var expenses = new List<ExpenseResponse>
    {
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-1),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Restaurant),
            Amount = 100,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Dinner",
            UserFullName = "Tony Stark"
        },
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-2),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Hotel),
            Amount = 200,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Hotel stay",
            UserFullName = "Tony Stark"
        }
    };
            _expenseService.GetExpensesByUser(userId, "date", "asc").Returns(expenses);

            var result = _controller.GetExpenses(userId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedExpenses = Assert.IsType<List<ExpenseResponse>>(okResult.Value);

            Assert.Equal(expenses.Count, returnedExpenses.Count);
            Assert.Equal(expenses[0].UserFullName, returnedExpenses[0].UserFullName);
            Assert.Equal(expenses[0].Type, returnedExpenses[0].Type);
            Assert.Equal(expenses[0].Currency, returnedExpenses[0].Currency);
        }

        [Fact]
        public void GetExpensesShouldReturnNotFoundWhenUserHasNoExpenses()
        {
            Guid userId = Guid.NewGuid();
            _expenseService.GetExpensesByUser(userId, "date", "asc").Returns(new List<ExpenseResponse>());

            var result = _controller.GetExpenses(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"No expenses found for user with ID {userId}.", notFoundResult.Value);
        }

        [Fact]
        public void GetExpensesShouldReturnNotFoundWhenUserIsNotFound()
        {
            Guid userId = Guid.NewGuid();
            _expenseService.When(x => x.GetExpensesByUser(userId, "date", "asc")).Do(x => { throw new UserNotFoundException(userId); });

            var result = _controller.GetExpenses(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"User with ID {userId} not found.", notFoundResult.Value);
        }

        [Fact]
        public void GetExpensesShouldReturnInternalServerErrorOnUnexpectedException()
        {
            var userId = Guid.NewGuid();
            _expenseService.When(x => x.GetExpensesByUser(userId, "date", "asc"))
                           .Do(x => { throw new Exception("Unexpected error"); });

            var result = _controller.GetExpenses(userId);
            var serverErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Unexpected error", serverErrorResult.Value);
        }

        [Fact]
        public void GetExpensesShouldReturnNotFoundWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _expenseService.When(x => x.GetExpensesByUser(userId, "date", "asc"))
                           .Do(x => { throw new UserNotFoundException(userId); });

            var result = _controller.GetExpenses(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"User with ID {userId} not found.", notFoundResult.Value);
        }

        [Fact]
        public void GetExpensesShouldReturnBadRequestWhenSortByParameterIsInvalid()
        {

            var userId = Guid.NewGuid();
            _expenseService.When(x => x.GetExpensesByUser(userId, "invalidSort", "asc"))
                           .Do(x => { throw new ArgumentException("Invalid sort parameter."); });
            var result = _controller.GetExpenses(userId, "invalidSort", "asc");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid sort parameter.", badRequestResult.Value);
        }

        [Fact]
        public void GetExpensesShouldReturnExpensesSortedByDateDesc()
        {
            Guid userId = Guid.NewGuid();
            var expenses = new List<ExpenseResponse>
    {
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-1),
            Type = "Restaurant",
            Amount = 100,
            Currency = "USD",
            Comment = "Dinner",
            UserFullName = "Tony Stark"
        },
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-2),
            Type = "Hotel",
            Amount = 200,
            Currency = "USD",
            Comment = "Hotel stay",
            UserFullName = "Tony Stark"
        }
    };

            _expenseService.GetExpensesByUser(userId, "date", "desc").Returns(expenses.OrderByDescending(e => e.Date).ToList());

            var result = _controller.GetExpenses(userId, "date", "desc");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedExpenses = Assert.IsType<List<ExpenseResponse>>(okResult.Value);

            Assert.Equal(expenses.Count, returnedExpenses.Count);
            Assert.True(returnedExpenses.First().Date >= returnedExpenses.Last().Date);
        }

        [Fact]
        public void GetExpensesShouldSortByDateWhenSortByParameterIsNotProvided()
        {
            Guid userId = Guid.NewGuid();
            var expenses = new List<ExpenseResponse>
    {
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-1),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Restaurant),
            Amount = 100,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Dinner",
            UserFullName = "Tony Stark"
        },
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-2),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Hotel),
            Amount = 200,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Hotel stay",
            UserFullName = "Tony Stark"
        }
    };

            _expenseService.GetExpensesByUser(userId, "date", "asc").Returns(expenses);
            var result = _controller.GetExpenses(userId);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedExpenses = Assert.IsType<List<ExpenseResponse>>(okResult.Value);

            Assert.Equal(expenses.Count, returnedExpenses.Count);
            Assert.Equal(expenses[0].Date, returnedExpenses[0].Date);
            Assert.Equal(expenses[1].Date, returnedExpenses[1].Date);
        }

        [Fact]
        public void GetExpensesShouldUseAscSortOrderWhenSortOrderParameterIsNotProvided()
        {
            Guid userId = Guid.NewGuid();
            var expenses = new List<ExpenseResponse>
    {
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-1),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Restaurant),
            Amount = 50,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Coffee",
            UserFullName = "Tony Stark"
        },
        new ExpenseResponse
        {
            Date = DateTime.Now.AddDays(-2),
            Type = Enum.GetName(typeof(ExpenseType), ExpenseType.Hotel),
            Amount = 200,
            Currency = Enum.GetName(typeof(Currency), Currency.USD),
            Comment = "Hotel stay",
            UserFullName = "Tony Stark"
        }
    };

            _expenseService.GetExpensesByUser(userId, "amount", "asc").Returns(expenses);
            var result = _controller.GetExpenses(userId, "amount");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedExpenses = Assert.IsType<List<ExpenseResponse>>(okResult.Value);

            Assert.Equal(2, returnedExpenses.Count);
            Assert.Equal(50, returnedExpenses[0].Amount);
            Assert.Equal(200, returnedExpenses[1].Amount);
        }

        [Fact]
        public void GetExpensesShouldReturnNotFoundWhenUserExistsButHasNoExpenses()
        {
            Guid userId = Guid.NewGuid();
            _expenseService.GetExpensesByUser(userId, "date", "asc").Returns(new List<ExpenseResponse>());
            var result = _controller.GetExpenses(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"No expenses found for user with ID {userId}.", notFoundResult.Value);
        }
    }
}
