using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bidster.Models.Products
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public string EventSlug { get; set; }
        public string EventName { get; set; }

        public string Slug { get; set; }
        [Display(Name = "Name")]
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Starting price")]
        public decimal StartingPrice { get; set; }
        [Display(Name = "Minimum bid amount")]
        public decimal MinimumBidAmount { get; set; }

        public bool HasBids { get; set; }

        public IFormFile ImageFile { get; set; }
        public string ImageFilename { get; set; }
        public string ImageUrl { get; set; }

        public IFormFile ThumbnailFile { get; set; }
        public string ThumbnailFilename { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}