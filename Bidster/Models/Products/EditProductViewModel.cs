using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Products
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public string EventSlug { get; set; }
        public string EventName { get; set; }

        public string Slug { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal MinimumBidAmount { get; set; }

        public bool HasBids { get; set; }
    }
}