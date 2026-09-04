using SubastaYa.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuctionService
    {
        // ver las subastas en la pagina principal
        Task<Auction?> GetByIdAsync(int id); // trae una subasta especifica
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync(); //muestra subastas activas

        // filtro completo para el catálogo (Estado, Categoría y Orden)
        Task<IEnumerable<Auction>> GetFilteredAuctionsAsync(string? state, int? categoryId, string? sortBy);

        // publica una nueva subasta
        Task<Auction> CreateAuctionAsync(Auction auction);

        // regla antisniping
        // Extiende 2 minutos si entra una puja en los últimos 60 segundos
        Task<bool> CheckAndApplyAntiSnipingAsync(int auctionId);

        // cierre automatico de subasta
        Task<IEnumerable<Auction>> GetExpiredAuctionsAsync(); // busca las subastas finalizadas

        // Cierra la subasta
        Task<bool> ProcessAuctionClosureAsync(int auctionId);

        //Panel de Usuario "Mis Actividades
        Task<IEnumerable<Auction>> GetAuctionsBySellerAsync(int sellerId); // muestra lo que el usuario pone en venta
        Task<IEnumerable<Auction>> GetAuctionsByBidderAsync(int buyerId); // subasta que el usuario participo
    }
}
