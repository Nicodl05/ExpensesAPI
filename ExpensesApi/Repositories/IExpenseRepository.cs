using ExpensesAPI.Domain;

namespace ExpensesAPI.Repositories
{
    public interface IExpenseRepository
    {
        void AddExpense(Expense expense);
        IEnumerable<Expense> GetExpensesByUser(Guid userId);
        bool CheckDuplicate(Expense expense);
    }
}
