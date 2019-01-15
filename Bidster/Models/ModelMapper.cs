using System;
using Bidster.Entities.Bids;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Models.Bids;
using Bidster.Models.Events;
using Bidster.Models.EventUsers;
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
            CreatedOn = evt.CreatedOn,
            HideBidderNames = evt.HideBidderNames,
            IsBiddingOpen = evt.IsBiddingOpen(DateTime.Now)
        };

        public static ProductModel ToProductModel(this Product product) => new ProductModel
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
            BidCount = product.BidCount,
            NextMinBidAmount = product.NextMinBidAmount,
            ImageFilename = product.ImageFilename,
            ThumbnailFilename = product.ThumbnailFilename
        };

        public static BidModel ToBidModel(this Bid bid) => new BidModel
        {
            Id = bid.Id,
            ProductId = bid.ProductId,
            UserId = bid.User.Id,
            UserName = bid.User.FullName,
            Timestamp = bid.Timestamp,
            Amount = bid.Amount
        };

        public static EventUserModel ToEventUserModel(this EventUser evtUser) => new EventUserModel
        {
            Id = evtUser.Id,
            EventId = evtUser.EventId,
            UserId = evtUser.User.Id,
            UserName = evtUser.User.FullName,
            UserEmail = evtUser.User.Email,
            IsAdmin = evtUser.IsAdmin,
            CreatedOn = evtUser.CreatedOn
        };
    }
}