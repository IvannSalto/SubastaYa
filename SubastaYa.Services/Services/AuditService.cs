using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using SubastaYa.Core.IRepositories;

namespace SubastaYa.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository;

    public AuctionService(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public async Task<Auction?> GetByIdAsync(int id)
    {
        return await _auctionRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
    {
        return await _auctionRepository.GetActiveAsync();
    }

    public async Task<IEnumerable<Auction>> GetFilteredAuctionsAsync(string? state, int? categoryId, string? sortBy)
    {
        return await _auctionRepository.GetFilteredAsync(state, categoryId, sortBy);
    }

    public async Task<Auction> CreateAuctionAsync(Auction auction)
    {
        // Reglas de negocio al crear
        auction.State = "Active";
        auction.StartDate = DateTime.UtcNow;

        await _auctionRepository.AddAsync(auction);
        await _auctionRepository.SaveChangesAsync();

        return auction;
    }

    public async Task<bool> CheckAndApplyAntiSnipingAsync(int auctionId)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId);

        if (auction == null || auction.State != "Active")
        {
            return false;
        }

        var timeRemaining = auction.EndDate - DateTime.UtcNow;

        // Si qedan 60 segundos o menos, se extinde 2 minutos
        if (timeRemaining.TotalSeconds > 0 && timeRemaining.TotalSeconds <= 60)
        {
            auction.EndDate = auction.EndDate.AddMinutes(2);

            _auctionRepository.Update(auction);
            await _auctionRepository.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<Auction>> GetExpiredAuctionsAsync()
    {
        return await _auctionRepository.GetExpiredAsync();
    }

    public async Task<bool> ProcessAuctionClosureAsync(int auctionId)
    {
        var auction = await _auctionRepository.GetByIdAsync(auctionId);

        if (auction == null || auction.State == "Closed")
        {
            return false;
        }

        // Buscamos la puja de mayor valor
        var highestBid = auction.Bids?
            .OrderByDescending(b => b.Amount)
            .FirstOrDefault();

        if (highestBid != null)
        {
            auction.State = "Closed";
            auction.WinnerId = highestBid.Buyer.Id; // Guardamos el ID del comprador
        }
        else
        {
            auction.State = "FinishedWithoutWinner";
            auction.WinnerId = null;
        }

        _auctionRepository.Update(auction);
        await _auctionRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Auction>> GetAuctionsBySellerAsync(int sellerId)
    {
        return await _auctionRepository.GetBySellerAsync(sellerId);
    }

    public async Task<IEnumerable<Auction>> GetAuctionsByBidderAsync(int buyerId)
    {
        return await _auctionRepository.GetByBidderAsync(buyerId);
    }
}