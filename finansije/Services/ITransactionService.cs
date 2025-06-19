using finansije.Models;

namespace finansije.Services
{
    public interface ITransactionService
    {
        public Task<int> AddCategoryAsync(string name);
        public Task<int> AddTransactionAsync(TransactionDto transaction);
        public Task<int> GetAccountBalanceAsync();
        public Task<List<TransactionDto>> GetTransactionsAsync(string type = null);
        public Task<List<TransactionDto>> GetLastMonthsTransactions(string type = null);
        public Task<int> EditTransactionAsync(TransactionDto transaction, int id);
        public Task<int> DeleteTransactionAsync(int id);
    }
}
