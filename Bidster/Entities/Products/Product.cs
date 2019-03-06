using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidster.Entities.Events;
using Bidster.Entities.Tenants;

namespace Bidster.Entities.Products
{
    public class Product 
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        public int EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; }

        [Required, MaxLength(100)]
        public string Slug { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal MinimumBidAmount { get; set; }

        public decimal CurrentBidAmount { get; set; }
        public int? CurrentHighBidUserId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? BuyItNowPrice { get; set; }
        public DateTime? PurchasedDate { get; set; }
        public int? PurchasedUserId { get; set; }
        public bool IsPurchased => PurchasedDate.HasValue;

        public int BidCount { get; set; }
        public bool HasBids => BidCount > 0;
        
        public decimal NextMinBidAmount => !HasBids ? CurrentBidAmount : CurrentBidAmount + MinimumBidAmount;

        [MaxLength(100)]
        public string ImageFilename { get; set; }
        [MaxLength(100)]
        public string ThumbnailFilename { get; set; }
    }
}