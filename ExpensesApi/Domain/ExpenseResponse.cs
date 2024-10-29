using ExpensesAPI.Domain;

public class ExpenseResponse
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Comment { get; set; }
    public string UserFullName { get; set; }
}
