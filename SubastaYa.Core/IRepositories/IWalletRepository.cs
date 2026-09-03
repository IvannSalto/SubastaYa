using SubastaYa.Core.Entities;

namespace SubastaYa.Core.IRepositories;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    public Task<Wallet?> GetByUserIdAsync(int userId);
}