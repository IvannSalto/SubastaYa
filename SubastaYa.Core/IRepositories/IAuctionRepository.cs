using SubastaYa.Core.Entities;

namespace SubastaYa.Core.IRepositories;

public interface IAuctionRepository : IGenericRepository<Auction>
{
    Task<IEnumerable<Auction>> GetActiveAsync();
    Task<IEnumerable<Auction>> GetFilteredAsync(string? state, int? categoryId, string? sortBy);
    Task<IEnumerable<Auction>> GetExpiredAsync();
    Task<IEnumerable<Auction>> GetBySellerAsync(int sellerId);
    Task<IEnumerable<Auction>> GetByBidderAsync(int buyerId);
    Task SaveChangesAsync();
}
