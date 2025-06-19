using System.Security.Claims;
using System.Security.Cryptography;
using finansije.Data;
using finansije.Entities;
using finansije.Models;
using Microsoft.EntityFrameworkCore;

namespace finansije.Services
{
    public class TransactionService(UsersContext context, ICurrentUserService currentUserService) : ITransactionService
    {
        public async Task<int> AddCategoryAsync(string name)
        {
            if (await context.Categories.AnyAsync(c => c.Name == name))
            {
                return -1;
            }
            var category = new Category()
            {
                Name = name
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return 0;
        }
        public async Task<int> AddTransactionAsync(TransactionDto transaction)
        {
            var userIdGuid = currentUserService.UserId;
            if (!await context.Categories.AnyAsync(c => c.Id == transaction.CategoryId))
                return -1;
            var newTransaction = new Transaction()
            {
                Type = transaction.Type,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId,
                DateTime = transaction.DateTime,
                Description = transaction.Description,
                UserId = userIdGuid
            };
            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync();
            return 0;
        }

        public async Task<int> GetAccountBalanceAsync()
        {
            var userId = currentUserService.UserId;
            var transactions = await context.Transactions
                .Where(t => t.UserId == userId).ToListAsync();

            int balance = transactions.Sum(t =>
            t.Type == "income" ? t.Amount : -t.Amount);
            return balance;
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync(string type = null)
        {
            var userId = currentUserService.UserId;
            List<Transaction> transactions;
            if (type != null)
            {
                transactions = await context.Transactions
                .Where(t => t.UserId == userId && t.Type == type)
                .ToListAsync();

            }
            else
            {
                transactions = await context.Transactions
                    .Where(t => t.UserId == userId).ToListAsync();
            }
            var result = transactions.Select(t => new TransactionDto()
            {
                Type = t.Type,
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                DateTime = t.DateTime,
                Description = t.Description
            }).ToList();
            return result;
        }
        public async Task<List<TransactionDto>> GetLastMonthsTransactions(string type = null)
        {
            var userId = currentUserService.UserId;
            List<Transaction> transactions;
            var today = DateTime.Today;
            var firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
            var lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1).Date.AddDays(1).AddTicks(-1);
            if (type != null)
            {
                transactions = await context.Transactions
                .Where(t => t.UserId == userId && t.Type == type && 
                t.DateTime >= firstDayOfLastMonth &&
                t.DateTime <= lastDayOfLastMonth)
                .ToListAsync();

            }
            else
            {
                transactions = await context.Transactions
                    .Where(t => t.UserId == userId &&
                t.DateTime >= firstDayOfLastMonth &&
                t.DateTime <= lastDayOfLastMonth).ToListAsync();
            }
            var result = transactions.Select(t => new TransactionDto()
            {
                Type = t.Type,
                Amount = t.Amount,
                CategoryId = t.CategoryId,
                DateTime = t.DateTime,
                Description = t.Description
            }).ToList();
            return result;
        }

        public async Task<int> EditTransactionAsync(TransactionDto transaction, int id)
        {
            var old = await context.Transactions.FindAsync(id);
            if (old == null)
                return -1;
            var currentUserId = currentUserService.UserId;
            if (old.UserId != currentUserId)
                return -2;
            old.Amount = transaction.Amount;
            old.CategoryId = transaction.CategoryId;
            old.DateTime = transaction.DateTime;
            old.Description = transaction.Description;
            old.Type = transaction.Type;
            await context.SaveChangesAsync();
            return 0;
        }

        public async Task<int> DeleteTransactionAsync(int id)
        {
            var transaction = await context.Transactions.FindAsync(id);
            if (transaction == null)
                return -1;
            var currentUserId = currentUserService.UserId;
            if (transaction.UserId != currentUserId)
                return -2;
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
            return 0;
        }
    }
}
