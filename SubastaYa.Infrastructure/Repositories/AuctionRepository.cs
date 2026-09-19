using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using SubastaYa.Core.Utils;
using SubastaYa.Infrastructure.Data;

namespace SubastaYa.Infrastructure.Repositories;

public class AuctionRepository : GenericRepository<Auction>,IAuctionRepository
{
    private readonly ApplicationDbContext _context;

    public AuctionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Auction>> GetActiveAsync()
    {
        return await _context.Auctions
            .AsNoTracking()
            .Where(a => a.State == "Active" && a.EndDate > ArgTime.Now)
            .OrderBy(a => a.EndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetFilteredAsync(string? state, int? categoryId, string? sortBy, string? search, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Auctions.Include(a => a.Bids).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(a => a.State == state);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }
        
        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Title.Contains(search) || a.Description.Contains(search));
        
        if (minPrice.HasValue && minPrice.Value > 0)
            query = query.Where(a => (a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice) >= minPrice.Value);

        if (maxPrice.HasValue && maxPrice.Value > 0)
            query = query.Where(a => (a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice) <= maxPrice.Value);

        query = sortBy?.ToLower() switch
        {
            "bid-asc" => query.OrderBy(a => a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice),
            "bid-desc" => query.OrderByDescending(a => a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice),
            "time-asc" => query.OrderBy(a => a.State == "Active" ? 0 : 1).ThenBy(a => a.EndDate),
            "newest" => query.OrderByDescending(a => a.StartDate),
            _ => query.OrderByDescending(a => a.StartDate)
        };

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetExpiredAsync()
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .Where(a => a.State == "Active" && a.EndDate <= ArgTime.Now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetBySellerAsync(int sellerId)
    {
        return await _context.Auctions
            .AsNoTracking()
            .Where(a => a.SellerId == sellerId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetByBidderAsync(int buyerId)
    {
        return await _context.Auctions
            .AsNoTracking()
            .Where(a => a.Bids.Any(b => b.Buyer.Id == buyerId))
            .OrderByDescending(a => a.EndDate)
            .ToListAsync();
    }
    
    public async Task<Auction> GetByIdWithBidsAsync(int auctionId)
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == auctionId);
    }
}