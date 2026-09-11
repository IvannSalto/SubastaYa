using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Core.Interfaces
{
    public interface IAuctionNotifier
    {
        // Avisa que alguien hizo una nueva puja
        Task BroadcastNewBidAsync(int auctionId, decimal newAmount);

        // Avisa que una subasta terminó
        Task BroadcastAuctionClosedAsync(int auctionId, int? winnerId);
    }
}
