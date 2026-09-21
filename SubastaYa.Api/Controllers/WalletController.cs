using Microsoft.AspNetCore.Mvc;
using SubastaYa.Api.Responses;
using SubastaYa.Core.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SubastaYa.Api.Extensions;
using SubastaYa.Core.IRepositories;

namespace SubastaYa.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionLedgerRepository _ledgerRepository;

        public WalletController(IWalletService walletService, IWalletRepository walletRepository, ITransactionLedgerRepository ledgerRepository)
        {
            _walletService = walletService;
            _walletRepository = walletRepository;
            _ledgerRepository = ledgerRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);

            return Ok(ApiResponse<object>.Ok(wallet, "Billetera obtenida correctamente."));
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
        {
            int userId = User.GetUserId();
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            
            await _walletService.DepositFundsAsync(wallet.Id, request.Amount);

            return Ok(ApiResponse<object>.Ok(null, $"Se depositaron ${request.Amount} exitosamente."));
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            int userId = User.GetUserId();
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);
            
            await _walletService.WithdrawFundsAsync(wallet.Id, request.Amount);

            return Ok(ApiResponse<object>.Ok(null, $"Se retiraron ${request.Amount} exitosamente."));
        }
        
        [HttpGet("user/{userId}/movements")]
        public async Task<IActionResult> GetMovements(int userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null) return NotFound(new { message = "Billetera no encontrada." });
            
            var movements = await _ledgerRepository.GetByWalletIdAsync(wallet.Id);
            
            return Ok(new { data = movements });
        }
    }

    // Objeto auxiliar para recibir los datos del frontend
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
    }
}