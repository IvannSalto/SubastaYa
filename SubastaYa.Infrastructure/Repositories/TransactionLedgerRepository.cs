using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using SubastaYa.Infrastructure.Data;

namespace SubastaYa.Infrastructure.Repositories
{
    public class TransactionLedgerRepository : ITransactionLedgerRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionLedgerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TransactionLedger transaction)
        {
            await _context.TransactionLedgers.AddAsync(transaction);
            
            await _context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<TransactionLedger>> GetByWalletIdAsync(int walletId)
        {
            return await _context.TransactionLedgers
                .AsNoTracking()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }
    }
}