using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Products
{
    public class ProductModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Slug { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal MinimumBidAmount { get; set; }

        public decimal CurrentBidAmount { get; set; }
        public int? CurrentHighBidUserId { get; set; }

        public int BidCount { get; set; }
    }
}