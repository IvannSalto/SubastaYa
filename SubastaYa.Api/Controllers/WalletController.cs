using Microsoft.AspNetCore.Mvc;
using SubastaYa.Api.Responses;
using SubastaYa.Core.Interfaces;
using System.Threading.Tasks;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(userId);

            return Ok(ApiResponse<object>.Ok(wallet, "Billetera obtenida correctamente."));
        }

        [HttpPost("{walletId}/deposit")]
        public async Task<IActionResult> Deposit(int walletId, [FromBody] TransactionRequest request)
        {
            await _walletService.DepositFundsAsync(walletId, request.Amount);

            return Ok(ApiResponse<object>.Ok(null, $"Se depositaron ${request.Amount} exitosamente."));
        }

        [HttpPost("{walletId}/withdraw")]
        public async Task<IActionResult> Withdraw(int walletId, [FromBody] TransactionRequest request)
        {
            await _walletService.WithdrawFundsAsync(walletId, request.Amount);

            return Ok(ApiResponse<object>.Ok(null, $"Se retiraron ${request.Amount} exitosamente."));
        }
    }

    // Objeto auxiliar para recibir los datos del frontend
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
    }
}