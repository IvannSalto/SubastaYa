using Microsoft.AspNetCore.Mvc;
using SubastaYa.Api.Responses;
using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SubastaYa.API.DTOs;
using SubastaYa.Api.Extensions;
using SubastaYa.Core.Utils;

namespace SubastaYa.API.Controllers
{
    [Authorize]
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
        
        [AllowAnonymous] //cualquiera va a poder ver las subastas activas
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var auctions = await _auctionService.GetActiveAuctionsAsync();
            return Ok(ApiResponse<object>.Ok(auctions, "Subastas activas listadas."));
        }
        
        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] string? state, 
            [FromQuery] int? categoryId, 
            [FromQuery] string? sortBy,
            [FromQuery] string? search,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var auctions = await _auctionService.GetFilteredAuctionsAsync(state, categoryId, sortBy, search, minPrice, maxPrice);
            return Ok(ApiResponse<object>.Ok(auctions, "Catálogo filtrado exitosamente."));
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuctionRequest request)
        {
            int sellerId = User.GetUserId();
            
            string initialState = request.StartDate > ArgTime.Now.AddMinutes(2) ? "Pending" : "Active";
            var auction = new Auction
            {
                Title = request.Title,
                Description = request.Description,
                BasePrice = request.BasePrice,
                
                StartDate = request.StartDate, 
                EndDate = request.EndDate,
                
                SellerId = sellerId,
                UrlImage = request.UrlImage,
                CategoryId = request.CategoryId,
                
                MinimumIncrement = 1000, 
                State = initialState,
                Bids = new List<Bid>() 
            };

            var createdAuction = await _auctionService.CreateAuctionAsync(auction); 
            return Ok(ApiResponse<object>.Ok(createdAuction, "Subasta creada exitosamente."));
        }

        [HttpPost("{auctionId}/bid")]
        public async Task<IActionResult> PlaceBid(int auctionId, [FromBody] BidRequest request)
        {
            try
            {
                int buyerId = User.GetUserId();

                // motor financiero
                await _auctionService.PlaceBidAsync(auctionId, buyerId, request.Amount);
                return Ok(ApiResponse<object>.Ok(null, "Puja realizada con éxito. El dinero fue retenido."));
            }
            catch (Exception ex) when (ex.Message.Contains("múltiples operaciones") || ex.Message.Contains("ConcurrencyConflict"))
            {
                return Conflict(ApiResponse<object>.Fail("Alguien mas intento pujar exactamente al mismo tiempo. Por favor actualiza la pagina y volve a intentarlo."));
            }
        }

        [HttpPost("{auctionId}/close")]
        public async Task<IActionResult> CloseAuction(int auctionId)
        {
            int currentUserId = User.GetUserId();
            // descontamos al ganador, pagamos al vendedor
            await _auctionService.ProcessAuctionClosureAsync(auctionId, currentUserId);
            return Ok(ApiResponse<object>.Ok(null, "Subasta cerrada. Fondos transferidos correctamente."));
        }
    }
}