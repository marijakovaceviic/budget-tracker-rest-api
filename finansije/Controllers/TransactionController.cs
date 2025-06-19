using Azure.Core;
using finansije.Models;
using finansije.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace finansije.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(ITransactionService service, IValidator<TransactionDto> validator) : ControllerBase
    {
        [Authorize(Roles = "admin")]
        [HttpPost("addCategory/{name}")]
        public async Task<IActionResult> AddCategory(string name)
        {
            var status = await service.AddCategoryAsync(name);
            if (status != 0)
                return BadRequest("A category with this name already exists");
            return Ok("Category has been successfully added");
        }

        [Authorize(Roles = "user")]
        [HttpPost("addTransaction")]
        public async Task<IActionResult> AddTransaction(TransactionDto transaction)
        {
            var validationResult = validator.Validate(transaction);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage });

                return BadRequest(errors);
            }
            var status = await service.AddTransactionAsync(transaction);
            if (status == -1)
                return BadRequest("Invalid category id");
            return Ok("Transaction has been successfully added");
        }

        [Authorize(Roles = "user")]
        [HttpGet("accountBalance")]
        public async Task<ActionResult<int>> AccountBalance()
        {
            return Ok(await service.GetAccountBalanceAsync());
        }

        [Authorize(Roles = "user")]
        [HttpGet("allIncome")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllIncomes()
        {
            return Ok(await service.GetTransactionsAsync("income"));
        }

        [Authorize(Roles = "user")]
        [HttpGet("allExpenses")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllExpenses()
        {
            return Ok(await service.GetTransactionsAsync("expense"));
        }

        [Authorize(Roles = "user")]
        [HttpGet("allTransactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllTransactions()
        {
            return Ok(await service.GetTransactionsAsync());
        }

        [Authorize(Roles = "user")]
        [HttpGet("allLastMonthsTransactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllLastMonthsTransactions()
        {
            return Ok(await service.GetLastMonthsTransactions());
        }

        [Authorize(Roles = "user")]
        [HttpGet("allLastMonthsIncome")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllLastMonthsIncome()
        {
            return Ok(await service.GetLastMonthsTransactions("income"));
        }

        [Authorize(Roles = "user")]
        [HttpGet("allLastMonthsExpenses")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllLastMonthsExpenses()
        {
            return Ok(await service.GetLastMonthsTransactions("expense"));
        }

        [Authorize(Roles = "user")]
        [HttpPut("editTransaction/{id}")]
        public async Task<IActionResult> EditTransaction(int id, TransactionDto transaction)
        {
            var status = await service.EditTransactionAsync(transaction, id);
            if (status == -1)
                return NotFound();
            if (status == -2)
                return BadRequest("You don't have permission to modify this transaction");
            return NoContent();
        }

        [Authorize(Roles = "user")]
        [HttpDelete("removeTransaction/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var status = await service.DeleteTransactionAsync(id);
            if (status == -1)
                return NotFound();
            if (status == -2)
                return BadRequest("You don't have permission to remove this transaction");
            return NoContent();
        }
    }
}
