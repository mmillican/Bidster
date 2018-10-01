using System.Threading.Tasks;

namespace Bidster.Services.Notifications
{
    public interface IBidService
    {
        Task<PlaceBidResult> PlaceBidAsync(PlaceBidRequest bidRequest);

        Task SendOutbidNotice(int eventId, int productId);

        Task SendItemWonNotice(int eventId, int productId);
    }
}
