using SubastaYa.Core.Entities;

namespace SubastaYa.Core.IRepositories
{
    public interface ITransactionLedgerRepository
    {
        Task AddAsync(TransactionLedger transaction);
        Task<IEnumerable<TransactionLedger>> GetByWalletIdAsync(int walletId);
    }
    
}