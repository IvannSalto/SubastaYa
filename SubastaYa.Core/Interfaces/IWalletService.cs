using SubastaYa.Core.Entities;
 
namespace SubastaYa.Core.Interfaces;

public interface IWalletService
{
    Task<Wallet> GetWalletAsync(int walletId);
    
    Task<Wallet> GetWalletByUserIdAsync(int userId);
    
    Task RetainFundsAsync(int walletId, decimal amount, int? auctionId = null);

    Task ReleaseFundsAsync(int walletId, decimal amount, int? auctionId = null);

    Task DeductFundsAsync(int walletId, decimal amount, int? auctionId = null);
    
    Task DepositFundsAsync(int walletId, decimal amount, int? auctionId = null);
    
    Task WithdrawFundsAsync(int walletId, decimal amount);
}