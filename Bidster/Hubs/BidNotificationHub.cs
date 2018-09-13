using System.Threading.Tasks;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Microsoft.AspNetCore.SignalR;

namespace Bidster.Hubs
{
    public class BidNotificationHub : Hub
    {
        public async Task SendBidNotification(Event evt, Product product)
        {
            // TODO: send to only users that have bid on this item
            await Clients.All.SendAsync("BidPlaced", evt, product);
        }
    }
}
