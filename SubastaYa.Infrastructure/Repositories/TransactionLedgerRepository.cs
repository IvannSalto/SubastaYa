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
    }
}