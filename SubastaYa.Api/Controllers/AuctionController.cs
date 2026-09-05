using Microsoft.AspNetCore.Mvc;
using SubastaYa.Api.Responses;
using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using System.Threading.Tasks;

namespace SubastaYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var auction = await _auctionService.GetByIdAsync(id);

            if (auction == null)
                return NotFound(ApiResponse<object>.Fail("Subasta no encontrada."));

            return Ok(ApiResponse<object>.Ok(auction, "Subasta obtenida con éxito."));
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var auctions = await _auctionService.GetActiveAuctionsAsync();
            return Ok(ApiResponse<object>.Ok(auctions, "Subastas activas listadas."));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] string? state, [FromQuery] int? categoryId, [FromQuery] string? sortBy)
        {
            var auctions = await _auctionService.GetFilteredAuctionsAsync(state, categoryId, sortBy);
            return Ok(ApiResponse<object>.Ok(auctions, "Catálogo filtrado exitosamente."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Auction auction)
        {
            var createdAuction = await _auctionService.CreateAuctionAsync(auction);
            return Ok(ApiResponse<object>.Ok(createdAuction, "Subasta publicada exitosamente."));
        }

        [HttpPost("{auctionId}/bid")]
        public async Task<IActionResult> PlaceBid(int auctionId, [FromBody] BidRequest request)
        {
            // motor financiero
            await _auctionService.PlaceBidAsync(auctionId, request.BuyerId, request.Amount);
            return Ok(ApiResponse<object>.Ok(null, "Puja realizada con éxito. El dinero fue retenido."));
        }

        [HttpPost("{auctionId}/close")]
        public async Task<IActionResult> CloseAuction(int auctionId)
        {
            // descontamos al ganador, pagamos al vendedor
            await _auctionService.ProcessAuctionClosureAsync(auctionId);
            return Ok(ApiResponse<object>.Ok(null, "Subasta cerrada. Fondos transferidos correctamente."));
        }
    }

    // Objeto auxiliar para recibir los datos de la puja
    public class BidRequest
    {
        public int BuyerId { get; set; }
        public decimal Amount { get; set; }
    }
}
