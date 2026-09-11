using Microsoft.AspNetCore.SignalR;
using SubastaYa.Api.Hubs;
using SubastaYa.Core.Interfaces;

namespace SubastaYa.Api.Services
{
    public class SignalRAuctionNotifier : IAuctionNotifier
    {
        private readonly IHubContext<AuctionHub> _hubContext;

        public SignalRAuctionNotifier(IHubContext<AuctionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastNewBidAsync(int auctionId, decimal newAmount)
        {
            // Le manda un mensaje llamado "ReceiveNewBid" SOLO a los que están mirando esa subasta
            await _hubContext.Clients.Group(auctionId.ToString())
                             .SendAsync("ReceiveNewBid", auctionId, newAmount);
        }

        public async Task BroadcastAuctionClosedAsync(int auctionId, int? winnerId)
        {
            await _hubContext.Clients.Group(auctionId.ToString())
                             .SendAsync("ReceiveAuctionClosed", auctionId, winnerId);
        }
    }
}