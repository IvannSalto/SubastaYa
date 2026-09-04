using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;

namespace SubastaYa.Services
{
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
            auction.State = "Active";
            auction.StartDate = DateTime.UtcNow;

            await _auctionRepository.AddAsync(auction);
            await _auctionRepository.SaveChangesAsync();

            return auction;
        }

        public async Task<bool> PlaceBidAsync(int auctionId, int buyerId, decimal amount)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);

            if (auction == null || auction.State != "Active" || auction.EndDate <= DateTime.UtcNow)
            {
                return false;
            }

            if (auction.SellerId == buyerId)
            {
                return false;
            }

            var highestBid = auction.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault();
            decimal minRequiredAmount = highestBid != null
                ? highestBid.Amount + auction.MinimumIncrement
                : auction.BasePrice;

            if (amount < minRequiredAmount)
            {
                return false;
            }

            var newBid = new Bid
            {
                AuctionId = auctionId,
                BuyerId = buyerId,
                Amount = amount,
                BidDate = DateTime.UtcNow
            };

            if (auction.Bids == null)
            {
                auction.Bids = new List<Bid>();
            }

            auction.Bids.Add(newBid);
            auction.Version++;

            var timeRemaining = auction.EndDate - DateTime.UtcNow;
            if (timeRemaining.TotalSeconds > 0 && timeRemaining.TotalSeconds <= 60)
            {
                auction.EndDate = auction.EndDate.AddMinutes(2);
            }

            _auctionRepository.Update(auction);
            await _auctionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckAndApplyAntiSnipingAsync(int auctionId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);

            if (auction == null || auction.State != "Active")
            {
                return false;
            }

            var timeRemaining = auction.EndDate - DateTime.UtcNow;

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

            var highestBid = auction.Bids?
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault();

            if (highestBid != null)
            {
                auction.State = "Closed";
                auction.WinnerId = highestBid.Buyer.Id;
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
}