using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Bidster.Hubs
{
    public class BidsterHub : Hub
    {
        public Task SendBidPlacedNotification(string productName, decimal amount)
        {
            return Clients.All.SendAsync("BidPlaced", productName, amount);
        }
    }
}
