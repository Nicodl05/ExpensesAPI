using Microsoft.AspNetCore.Mvc;
using ExpensesAPI.Domain;
using ExpensesAPI.Services;
using ExpensesAPI.Exceptions;

namespace ExpensesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public ActionResult<Expense> CreateExpense([FromBody] Expense expense)
        {
            try
            {
                _expenseService.AddExpense(expense);
                return Ok("Expense created successfully.");
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidExpenseDateException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DuplicateExpenseException ex)
            {
                return Conflict(ex.Message);
            }
            catch (InvalidAmountException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCurrencyException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<ExpenseResponse>> GetExpenses(Guid userId, [FromQuery] string sortBy = "date", [FromQuery] string sortOrder = "asc")
        {
            try
            {
                var expenses = _expenseService.GetExpensesByUser(userId, sortBy, sortOrder);
                if (!expenses.Any())
                {
                    return NotFound($"No expenses found for user with ID {userId}.");
                }
                return Ok(expenses);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
