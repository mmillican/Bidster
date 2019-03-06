using System;
using Bidster.Entities.Bids;
using Bidster.Entities.Events;
using Bidster.Entities.Products;
using Bidster.Entities.Tenants;
using Bidster.Entities.Users;
using Bidster.Models.Bids;
using Bidster.Models.Events;
using Bidster.Models.EventUsers;
using Bidster.Models.Products;
using Bidster.Models.Tenants;
using Bidster.Models.Users;

namespace Bidster.Models
{
    public static class ModelMapper
    {
        public static UserModel ToUserModel(this User user) => new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Address = user.Address,
            Address2 = user.Address2,
            City = user.City,
            State = user.State,
            PostalCode = user.PostalCode
        };

        public static TenantUserModel ToModel(this TenantUser tenantUser) => new TenantUserModel
        {
            Id = tenantUser.Id,
            TenantId = tenantUser.TenantId,
            UserId = tenantUser.UserId,
            User = tenantUser.User.ToUserModel(),
            AddedOn = tenantUser.AddedOn,
            IsAdmin = tenantUser.IsAdmin
        };

        public static TenantModel ToTenantModel(this Tenant tenant) => new TenantModel
        {
            Id = tenant.Id,
            Name = tenant.Name,
            HostNames = tenant.HostNames,
            IsDisabled = tenant.IsDisabled
        };

        public static EventModel ToEventModel(this Event evt) => new EventModel
        {
            Id = evt.Id,
            Slug = evt.Slug,
            Name = evt.Name,
            Description = evt.Description,
            StartOn = evt.StartOn,
            EndOn = evt.EndOn,
            DisplayOn = evt.DisplayOn,
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
            BuyItNowPrice = product.BuyItNowPrice,
            PurchasedDate = product.PurchasedDate,
            PurchasedUserId = product.PurchasedUserId,
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