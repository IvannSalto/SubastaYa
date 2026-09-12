using SubastaYa.Core.Entities;
using SubastaYa.Core.Interfaces;
using SubastaYa.Core.Utils;

namespace SubastaYa.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IWalletService _walletService;
        private readonly IAuditService _auditService;
        public AuctionService(IAuctionRepository auctionRepository, IWalletService wallerService, IAuditService auditService)
        {
            _auctionRepository = auctionRepository;
            _walletService = wallerService;
            _auditService = auditService;
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
            auction.StartDate = ArgTime.Now;

            await _auctionRepository.AddAsync(auction);
            await _auctionRepository.SaveChangesAsync();

            //Crea auditoria al crear la subasta
            await _auditService.RegisterLogAsync(
                entity: nameof(Auction),
                entityId: auction.Id,
                action: "Create",
                userId: auction.SellerId,
                detail: new
                {
                    auction.Title,
                    auction.BasePrice,
                    auction.MinimumIncrement,
                    auction.EndDate
                }
            );
            return auction;
        }

        public async Task<bool> PlaceBidAsync(int auctionId, int buyerId, decimal amount)
        {
            var auction = await _auctionRepository.GetByIdWithBidsAsync(auctionId);
            
            // ----------------Validaciones------------------
            if (auction == null)
                throw new InvalidOperationException("La subasta no existe.");

            if (auction.State != "Active" || auction.EndDate <= ArgTime.Now)
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
            await _walletService.RetainFundsAsync(buyerWallet.Id, amount, auctionId);
            
            if (highestBid != null) // si ya habia un buyer antes, le devolvemos la plata retenida
            {
                var previousBidderWallet = await _walletService.GetWalletByUserIdAsync(highestBid.BuyerId);
                await _walletService.ReleaseFundsAsync(previousBidderWallet.Id, highestBid.Amount, auctionId);
            }
            
            var newBid = new Bid
            {
                AuctionId = auctionId,
                BuyerId = buyerId,
                Amount = amount,
                BidDate = ArgTime.Now
            };

            if (auction.Bids == null)
                auction.Bids = new List<Bid>();
            
            auction.Bids.Add(newBid);
            auction.Version++;

            ApplyAntiSniping(auction);

            await _auctionRepository.UpdateAsync(auction);

            try
            {
                await _auctionRepository.SaveChangesAsync();
                //registra auditoria al realizar una puja exitosa
                await _auditService.RegisterLogAsync(
                    entity: nameof(Auction),
                    entityId: auctionId,
                    action: "PlaceBid",
                    userId: buyerId,
                    detail: new
                    {
                        BidAmount = amount,
                        PreviousHighestBid = highestBid?.Amount,
                        NewEndDate = auction.EndDate
                    }
                );

                return true;
            }
            catch (InvalidOperationException ex) when (ex.Message == "ConcurrencyConflict")
            {
                await _walletService.ReleaseFundsAsync(buyerWallet.Id, amount, auctionId); //si hubo un error de concurrencia le devolvemos la plata
                throw new Exception("Otra persona realizó una puja en el mismo milisegundo. Tu saldo fue devuelto, intentá de nuevo.");
            }
        }

        public async Task<IEnumerable<Auction>> GetExpiredAuctionsAsync()
        {
            return await _auctionRepository.GetExpiredAsync();
        }

        public async Task<bool> ProcessAuctionClosureAsync(int auctionId, int? currentUserId = null)
        {
            var auction = await _auctionRepository.GetByIdWithBidsAsync(auctionId);
         
            if (auction == null)
                throw new InvalidOperationException("La subasta no existe.");

            if (auction.State == "Closed" || auction.State == "FinishedWithoutWinner")
                throw new InvalidOperationException("La subasta ya se encuentra cerrada.");

            if (currentUserId.HasValue && auction.SellerId != currentUserId.Value)
                throw new UnauthorizedAccessException("Operación denegada: Solo el creador puede cerrarla.");

            var highestBid = auction.Bids?
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault();

            if (highestBid != null)
            {
                auction.State = "Closed";
                auction.WinnerId = highestBid.BuyerId;
                var winnerWallet = await _walletService.GetWalletByUserIdAsync(highestBid.BuyerId); //le cobramos al ganador
                await _walletService.DeductFundsAsync(winnerWallet.Id, highestBid.Amount, auctionId);
                
                var sellerWallet = await _walletService.GetWalletByUserIdAsync(auction.SellerId); //le pagamos al vendedor
                await _walletService.DepositFundsAsync(sellerWallet.Id, highestBid.Amount, auctionId);
            }
            else
            {
                auction.State = "FinishedWithNoWinner";
                auction.WinnerId = null;
            }

            await _auctionRepository.UpdateAsync(auction);
            await _auctionRepository.SaveChangesAsync();

            //Registra auditoria al cerrar una subasta
            await _auditService.RegisterLogAsync(
                entity: nameof(Auction),
                entityId: auctionId,
                action: "Close",
                userId: currentUserId ?? auction.SellerId,
                detail: new
                {
                    FinalState = auction.State,
                    WinnerId = auction.WinnerId,
                    WinningAmount = highestBid?.Amount
                }
            );
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
            var timeRemaining = auction.EndDate - ArgTime.Now;
            if (timeRemaining.TotalSeconds > 0 && timeRemaining.TotalSeconds <= 60)
            {
                auction.EndDate = auction.EndDate.AddMinutes(2);
            }
        }
    }
}