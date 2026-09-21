using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubastaYa.Core.Entities;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuctionService
    {
        Task<Auction?> GetByIdAsync(int id);
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();

        Task<IEnumerable<Auction>> GetFilteredAuctionsAsync(string? state, int? categoryId, string? sortBy, string? search, decimal? maxPrice, decimal? minPrice);

        Task<Auction> CreateAuctionAsync(Auction auction);

        Task<bool> PlaceBidAsync(int auctionId, int buyerId, decimal amount);
        
        Task<IEnumerable<Auction>> GetExpiredAuctionsAsync(); 

        Task<bool> ProcessAuctionClosureAsync(int auctionId, int? currentUserId = null);

        // Panel de Usuario "Mis Actividades"
        Task<IEnumerable<Auction>> GetAuctionsBySellerAsync(int sellerId);
        Task<IEnumerable<Auction>> GetAuctionsByBidderAsync(int buyerId);
    }
}