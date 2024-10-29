using ExpensesAPI.Domain;

public interface IExpenseService
{
    void AddExpense(Expense expense);
    IEnumerable<ExpenseResponse> GetExpensesByUser(Guid userId, string sortBy, string sortOrder);
}
