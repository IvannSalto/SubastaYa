using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;

namespace SubastaYa.Core.Services;

public class WalletService : IWalletService
{
    public Task<Wallet> GetWalletAsync(int walletId)
    {
        throw new NotImplementedException();
    }

    public Task RetainFundsAsync(int walletId, decimal amount)
    {
        throw new NotImplementedException();
    }

    public Task ReleaseFundsAsync(int walletId, decimal amount)
    {
        throw new NotImplementedException();
    }

    public Task DeductFundsAsync(int walletId, decimal amount)
    {
        throw new NotImplementedException();
    }

    public Task DepositFundsAsync(int walletId, decimal amount)
    {
        throw new NotImplementedException();
    }

    public Task WithdrawFundsAsync(int walletId, decimal amount)
    {
        throw new NotImplementedException();
    }
}