using ExpensesAPI.Domain;
using ExpensesAPI.Exceptions;
using ExpensesAPI.Repositories;

namespace ExpensesAPI.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;

        public ExpenseService(IExpenseRepository expenseRepository, IUserRepository userRepository)
        {
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
        }

        public void AddExpense(Expense expense)
        {
            var existingUser = _userRepository.GetUserByDetails(expense.User.FirstName, expense.User.LastName, expense.User.Currency);

            if (existingUser == null)
            {
                var newUser = new User
                {
                    FirstName = expense.User.FirstName,
                    LastName = expense.User.LastName,
                    Currency = expense.User.Currency
                };
                _userRepository.AddUser(newUser);
                existingUser = newUser;
            }
            expense.UserId = existingUser.Id;
            expense.User = null; // Needs to be null because it'll be resaved otherwise
            if (_expenseRepository.CheckDuplicate(expense))
            {
                throw new DuplicateExpenseException(expense.Date, expense.Amount);
            }
            _expenseRepository.AddExpense(expense);
        }

        public IEnumerable<ExpenseResponse> GetExpensesByUser(Guid userId, string sortBy = "date", string sortOrder = "asc")
        {
            var expenses = _expenseRepository.GetExpensesByUser(userId);
            var user = _userRepository.GetUserById(userId);

            if (user == null)
                throw new UserNotFoundException(userId);

            var sortedExpenses = sortBy.ToLower() switch
            {
                "amount" => sortOrder.ToLower() == "desc" ? expenses.OrderByDescending(e => e.Amount) : expenses.OrderBy(e => e.Amount),
                _ => sortOrder.ToLower() == "desc" ? expenses.OrderByDescending(e => e.Date) : expenses.OrderBy(e => e.Date)
            };

            return sortedExpenses.Select(expense => new ExpenseResponse
            {
                Date = expense.Date,
                Type = Enum.GetName(typeof(ExpenseType), expense.Type),
                Amount = expense.Amount,
                Currency = Enum.GetName(typeof(Currency), expense.Currency),
                Comment = expense.Comment,
                UserFullName = $"{user.FirstName} {user.LastName}"
            }).ToList();
        }


    }
}
