using ExpensesAPI.Data;
using ExpensesAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAPI.Repositories.Tests
{
    public class ExpenseRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly AppDbContext _context;
        private readonly ExpenseRepository _expenseRepository;

        public ExpenseRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(_options);
            _expenseRepository = new ExpenseRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void AddShouldAddExpenseWhenValidExpense()
        {
            _context.Expenses.RemoveRange(_context.Expenses);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            var user = new User
            {
                FirstName = "Tony",
                LastName = "Stark",
                Currency = Currency.USD
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var expense = new Expense(
                date: DateTime.Now.AddDays(-1),
                type: ExpenseType.Restaurant,
                amount: 100,
                currency: Currency.USD,
                comment: "Dinner",
                user: user
            );
            _expenseRepository.AddExpense(expense);
            _context.SaveChanges();

            Assert.Single(_context.Expenses);
            var storedExpense = _context.Expenses.Include(e => e.User).First();
            Assert.Equal(expense.Amount, storedExpense.Amount);
            Assert.Equal(expense.Comment, storedExpense.Comment);
            Assert.Equal(expense.Currency, storedExpense.Currency);
            Assert.Equal(user.Id, storedExpense.UserId);
        }

        [Fact]
        public void CheckDuplicateShouldReturnTrueWhenDuplicateExpenseExists()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now, ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);
            _context.Expenses.Add(expense);
            _context.SaveChanges();

            var result = _expenseRepository.CheckDuplicate(expense);
            Assert.True(result);
        }

        [Fact]
        public void GetExpensesByUserShouldReturnOnlyUserExpenses()
        {
            var user1 = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var user2 = new User { FirstName = "Steve", LastName = "Rogers", Currency = Currency.USD };
            var expense1 = new Expense(DateTime.Now, ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user1);
            var expense2 = new Expense(DateTime.Now, ExpenseType.Hotel, 200, Currency.USD, "Hotel stay", user2);

            _context.Expenses.AddRange(expense1, expense2);
            _context.SaveChanges();

            var result = _expenseRepository.GetExpensesByUser(user1.Id);
            Assert.Single(result);
            Assert.Equal(expense1, result.First());
        }

        [Fact]
        public void CheckDuplicateShouldReturnTrueWhenExpenseIsDuplicate()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            _context.Users.Add(user);
            _context.SaveChanges();
            var userId = user.Id;

            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);
            _context.Expenses.Add(expense);
            _context.SaveChanges();

            var duplicateExpense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user)
            {
                UserId = userId
            };
            var isDuplicate = _expenseRepository.CheckDuplicate(duplicateExpense);
            Assert.True(isDuplicate);
        }

        [Fact]
        public void CheckDuplicateShouldReturnFalseWhenAmountIsDifferent()
        {
            var user = new User { FirstName = "Tony", LastName = "Stark", Currency = Currency.USD };
            var expense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 100, Currency.USD, "Dinner", user);

            _context.Expenses.Add(expense);
            _context.SaveChanges();

            var nonDuplicateExpense = new Expense(DateTime.Now.AddDays(-1), ExpenseType.Restaurant, 150, Currency.USD, "Dinner", user);
            var isDuplicate = _expenseRepository.CheckDuplicate(nonDuplicateExpense);
            Assert.False(isDuplicate);
        }
    }

}
