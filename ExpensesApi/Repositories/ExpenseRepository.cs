using ExpensesAPI.Data;
using ExpensesAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAPI.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;

        public ExpenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddExpense(Expense expense)
        {
            _context.Expenses.Add(expense);
            _context.SaveChanges();
        }

        public IEnumerable<Expense> GetExpensesByUser(Guid userId)
        {
            return _context.Expenses
                           .Include(e => e.User)
                           .Where(e => e.UserId == userId)
                           .ToList();
        }
        public bool CheckDuplicate(Expense expense)
        {
            return _context.Expenses.Any(e =>
                e.UserId == expense.UserId &&
                e.Date.Date == expense.Date.Date &&
                e.Amount == expense.Amount &&
                e.Type == expense.Type);
        }
    }
}
