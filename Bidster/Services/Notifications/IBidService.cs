using System.Threading.Tasks;

namespace Bidster.Services.Notifications
{
    public interface IBidService
    {
        Task<PlaceBidResult> PlaceBidAsync(PlaceBidRequest bidRequest);


        Task SendItemWonNoticeAsync(int eventId, int productId);
    }
}
