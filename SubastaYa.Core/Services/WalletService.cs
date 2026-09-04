using System.Data;
using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using SubastaYa.Core.IRepositories;

namespace SubastaYa.Core.Services;

public class WalletService : IWalletService
{
    
    private readonly IWalletRepository _walletRepo;

    public WalletService(IWalletRepository walletRepo)
    {
        _walletRepo = walletRepo;
    }
    public async Task<Wallet> GetWalletAsync(int walletId)
    {
        var wallet = await _walletRepo.GetByIdAsync(walletId);
        if (wallet == null)
        {
            throw new Exception("Wallet not found.");
        }
        
        return wallet;
    }
    
    public async Task<Wallet> GetWalletByUserIdAsync(int userId)
    {
        var wallet = await _walletRepo.GetByUserIdAsync(userId);
    
        if (wallet == null)
        {
            throw new InvalidOperationException("No se encontró una billetera para este usuario.");
        }
    
        return wallet;
    }

    public async Task RetainFundsAsync(int walletId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("El saldo a retener debe ser mayor a 0");
        
        var wallet = await GetWalletAsync(walletId);
        
        if (wallet == null)
            throw new KeyNotFoundException("Wallet no encontrada.");

        if (wallet.AvailableBalance < amount)
            throw new InvalidOperationException("Saldo disponible insuficiente.");
        
        wallet.AvailableBalance -= amount;
        wallet.BalanceHeld += amount;
        await UpdateWalletAsync(wallet);
    }

    public async Task ReleaseFundsAsync(int walletId, decimal amount)
    { 
        if (amount <= 0)
            throw new ArgumentException("El monto a liberar debe ser mayor a cero.");

        var wallet = await GetWalletAsync(walletId);

        if (wallet.BalanceHeld < amount)
        {
            throw new InvalidOperationException("No hay suficientes fondos retenidos para liberar.");
        }

        wallet.BalanceHeld -= amount;
        wallet.AvailableBalance += amount;
        await UpdateWalletAsync(wallet);
    }

    public async Task DeductFundsAsync(int walletId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("El monto a descontar debe ser mayor a cero.");

        var wallet = await GetWalletAsync(walletId);

        if (wallet.BalanceHeld < amount)
        {
            throw new InvalidOperationException("Fondos retenidos insuficientes para realizar el cobro.");
        }

        wallet.BalanceHeld -= amount;
        await UpdateWalletAsync(wallet);
    }

    public async Task DepositFundsAsync(int walletId, decimal amount)
    {
        if(amount <= 0 )
            throw new ArgumentException("El monto a depositar debe ser mayor a cero.");
        
        var wallet = await GetWalletAsync(walletId);
        
        if (wallet == null)
            throw new KeyNotFoundException("Wallet no encontrada.");
        
        wallet.AvailableBalance += amount;
        await UpdateWalletAsync(wallet);
    }

    public async Task WithdrawFundsAsync(int walletId, decimal amount)
    {
        if(amount <= 0)
            throw new ArgumentException("El monto a retirar debe ser mayor a cero.");
        
        var wallet = await GetWalletAsync(walletId);
        
        if (wallet == null)
            throw new KeyNotFoundException("Wallet no encontrada.");
        
        if(amount > wallet.AvailableBalance)
            throw new InvalidOperationException("El monto a retirar excede tu saldo actual, probá con otro importe.");
        
        wallet.AvailableBalance -= amount;
        await UpdateWalletAsync(wallet);
    }

    private async Task UpdateWalletAsync(Wallet wallet) //metodo para controlar las concurrencias y los posibles doble click... DRY
    {
        wallet.Version = Guid.NewGuid();

        try
        {
            await _walletRepo.UpdateAsync(wallet);
        }
        catch (InvalidOperationException ex) when (ex.Message == "ConcurrencyConflict")
        {
            throw new Exception("Error procesando la transacción: múltiples operaciones simultáneas. Intentá de nuevo.");
        }
    }
}