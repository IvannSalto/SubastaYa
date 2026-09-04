using SubastaYa.Core.Entities;
 
namespace SubastaYa.Core.Interfaces;

public interface IWalletService
{
    Task<Wallet> GetWalletAsync(int walletId);
    
    Task<Wallet> GetWalletByUserIdAsync(int userId);
    
    Task RetainFundsAsync(int walletId, decimal amount);

    Task ReleaseFundsAsync(int walletId, decimal amount);

    Task DeductFundsAsync(int walletId, decimal amount);
    
    Task DepositFundsAsync(int walletId, decimal amount);
    
    Task WithdrawFundsAsync(int walletId, decimal amount);
}