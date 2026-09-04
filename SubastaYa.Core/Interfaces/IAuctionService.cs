using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Core.Entities;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuctionService
    {
        // Ver las subastas en la página principal
        Task<Auction?> GetByIdAsync(int id); // Trae una subasta específica
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync(); // Muestra subastas activas

        // Filtro completo para el catálogo (Estado, Categoría y Orden)
        Task<IEnumerable<Auction>> GetFilteredAuctionsAsync(string? state, int? categoryId, string? sortBy);

        // Publica una nueva subasta
        Task<Auction> CreateAuctionAsync(Auction auction);

        // Realizar una puja (valida monto, vendedor, concurrencia y aplica anti-sniping)
        Task<bool> PlaceBidAsync(int auctionId, int buyerId, decimal amount);

        // Regla antisniping
        // Extiende 2 minutos si entra una puja en los últimos 60 segundos
        Task<bool> CheckAndApplyAntiSnipingAsync(int auctionId);

        // Cierre automático de subasta
        Task<IEnumerable<Auction>> GetExpiredAuctionsAsync(); // Busca las subastas finalizadas

        // Cierra la subasta
        Task<bool> ProcessAuctionClosureAsync(int auctionId);

        // Panel de Usuario "Mis Actividades"
        Task<IEnumerable<Auction>> GetAuctionsBySellerAsync(int sellerId); // Muestra lo que el usuario pone en venta
        Task<IEnumerable<Auction>> GetAuctionsByBidderAsync(int buyerId); // Subastas en las que el usuario participó
    }
}