using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using SubastaYa.Infrastructure.Data;

namespace SubastaYa.Infrastructure.Repositories;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    
    public WalletRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId);
    }
}