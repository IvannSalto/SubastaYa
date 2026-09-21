using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuctionNotifier
    {
        Task BroadcastNewBidAsync(int auctionId, decimal newAmount);

        Task BroadcastAuctionClosedAsync(int auctionId, int? winnerId);
    }
}
