using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;

namespace SubastaYa.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IWalletService _walletService;

        public AuctionService(IAuctionRepository auctionRepository, IWalletService wallerService)
        {
            _auctionRepository = auctionRepository;
            _walletService = wallerService;
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
            
            // ----------------Validaciones------------------
            if (auction == null)
                throw new InvalidOperationException("La subasta no existe.");

            if (auction.State != "Active" || auction.EndDate <= DateTime.UtcNow)
                throw new InvalidOperationException("La subasta ya no se encuentra activa.");

            if (auction.SellerId == buyerId)
                throw new InvalidOperationException("No podés pujar por tu propio producto.");
            
            var highestBid = auction.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault(); 
            decimal minRequiredAmount = highestBid != null      //seteamos valor minimo para pujar
                ? highestBid.Amount + auction.MinimumIncrement
                : auction.BasePrice;

            if (amount < minRequiredAmount)
                throw new InvalidOperationException($"El monto debe ser de al menos ${minRequiredAmount}.");

            var buyerWallet = await _walletService.GetWalletByUserIdAsync(buyerId); //Delego la tarea a wallet, eso lo va a manejar walletService
            await _walletService.RetainFundsAsync(buyerWallet.Id, amount);
            
            if (highestBid != null) // si ya habia un buyer antes, le devolvemos la plata retenida
            {
                var previousBidderWallet = await _walletService.GetWalletByUserIdAsync(highestBid.BuyerId);
                await _walletService.ReleaseFundsAsync(previousBidderWallet.Id, highestBid.Amount);
            }
            
            var newBid = new Bid
            {
                AuctionId = auctionId,
                BuyerId = buyerId,
                Amount = amount,
                BidDate = DateTime.UtcNow
            };

            if (auction.Bids == null)
                auction.Bids = new List<Bid>();
            
            auction.Bids.Add(newBid);
            auction.Version++;

            ApplyAntiSniping(auction);

            _auctionRepository.Update(auction);

            try
            {
                await _auctionRepository.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException ex) when (ex.Message == "ConcurrencyConflict")
            {
                await _walletService.ReleaseFundsAsync(buyerWallet.Id, amount); //si hubo un error de concurrencia le devolvemos la plata
                throw new Exception("Otra persona realizó una puja en el mismo milisegundo. Tu saldo fue devuelto, intentá de nuevo.");
            }
        }

        public async Task<IEnumerable<Auction>> GetExpiredAuctionsAsync()
        {
            return await _auctionRepository.GetExpiredAsync();
        }

        public async Task<bool> ProcessAuctionClosureAsync(int auctionId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
         
            if (auction == null)
                throw new InvalidOperationException("La subasta no existe.");

            if (auction.State == "Closed" || auction.State == "FinishedWithoutWinner")
                throw new InvalidOperationException("La subasta ya se encuentra cerrada.");
            

            var highestBid = auction.Bids?
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault();

            if (highestBid != null)
            {
                auction.State = "Closed";
                auction.WinnerId = highestBid.Buyer.Id;
                var winnerWallet = await _walletService.GetWalletByUserIdAsync(highestBid.BuyerId); //le cobramos al ganador
                await _walletService.DeductFundsAsync(winnerWallet.Id, highestBid.Amount);
                
                var sellerWallet = await _walletService.GetWalletByUserIdAsync(auction.SellerId); //le pagamos al vendedor
                await _walletService.DepositFundsAsync(sellerWallet.Id, highestBid.Amount);
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
        
        private void ApplyAntiSniping(Auction auction)
        {
            var timeRemaining = auction.EndDate - DateTime.UtcNow;
            if (timeRemaining.TotalSeconds > 0 && timeRemaining.TotalSeconds <= 60)
            {
                auction.EndDate = auction.EndDate.AddMinutes(2);
            }
        }
    }
}