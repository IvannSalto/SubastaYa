using Microsoft.EntityFrameworkCore;
using SubastaYa.Core.Entities;
using SubastaYa.Core.IRepositories;
using SubastaYa.Infrastructure.Data;

namespace SubastaYa.Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly ApplicationDbContext _context;

    public AuctionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Auction?> GetByIdAsync(int id)
    {
        return await _context.Auctions
            .Include(a => a.Category)
            .Include(a => a.Bids)
                .ThenInclude(b => b.Buyer)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IReadOnlyList<Auction>> GetAllAsync()
    {
        return await _context.Auctions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Auction entity)
    {
        await _context.Auctions.AddAsync(entity);
    }

    public void Update(Auction entity)
    {
        _context.Auctions.Update(entity);
    }

    public void Delete(Auction entity)
    {
        _context.Auctions.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Auction>> GetActiveAsync()
    {
        return await _context.Auctions
            .AsNoTracking()
            .Where(a => a.State == "Active" && a.EndDate > DateTime.UtcNow)
            .OrderBy(a => a.EndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetFilteredAsync(string? state, int? categoryId, string? sortBy)
    {
        var query = _context.Auctions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(a => a.State == state);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.BasePrice),
            "price_desc" => query.OrderByDescending(a => a.BasePrice),
            "ending_soon" => query.OrderBy(a => a.EndDate),
            "newest" => query.OrderByDescending(a => a.StartDate),
            _ => query.OrderByDescending(a => a.StartDate)
        };

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Auction>> GetExpiredAsync()
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .Where(a => a.State == "Active" && a.EndDate <= DateTime.UtcNow)
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
}
