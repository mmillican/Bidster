using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Models.Events;
using Bidster.Models.Products;

namespace Bidster.Models
{
    public static class ModelMapper
    {
        public static EventModel ToEventModel(this Event evt) => new EventModel
        {
            Id = evt.Id,
            Slug = evt.Slug,
            Name = evt.Name,
            StartOn = evt.StartOn,
            EndOn = evt.EndOn,
            OwnerId = evt.OwnerId,
            CreatedOn = evt.CreatedOn
        };

        public static ProductModel ToProductModel(Product product) => new ProductModel
        {
            Id = product.Id,
            EventId = product.EventId,
            Slug = product.Slug,
            Name = product.Name,
            Description = product.Description,
            StartingPrice = product.StartingPrice,
            MinimumBidAmount = product.MinimumBidAmount,
            CurrentBidAmount = product.CurrentBidAmount,
            CurrentHighBidUserId = product.CurrentHighBidUserId,
            BidCount = product.BidCount
        };
    }
}