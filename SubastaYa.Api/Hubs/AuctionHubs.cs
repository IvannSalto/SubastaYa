using Microsoft.AspNetCore.SignalR;

namespace SubastaYa.Api.Hubs
{
    public class AuctionHub : Hub
    {
        // Acá el Frontend se va a suscribir a la "sala" de una subasta específica
        public async Task JoinAuctionGroup(string auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
        }
    }
}