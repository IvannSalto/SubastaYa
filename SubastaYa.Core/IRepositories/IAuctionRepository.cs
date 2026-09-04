using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;

public interface IAuctionRepository : IGenericRepository<Auction>
{
    // agrega estos métodos genéricos básicos que necesita el servicio:
    Task<IEnumerable<Auction>> GetActiveAsync();
    Task<IEnumerable<Auction>> GetFilteredAsync(string? state, int? categoryId, string? sortBy);
    Task<IEnumerable<Auction>> GetExpiredAsync();
    Task<IEnumerable<Auction>> GetBySellerAsync(int sellerId);
    Task<IEnumerable<Auction>> GetByBidderAsync(int buyerId);
    Task SaveChangesAsync();
}