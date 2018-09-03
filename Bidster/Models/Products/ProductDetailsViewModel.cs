using System.Collections.Generic;
using Bidster.Models.Bids;
using Bidster.Models.Events;

namespace Bidster.Models.Products
{
    public class ProductDetailsViewModel
    {
        public ProductModel Product { get; set; }

        public EventModel Event { get; set; }

        public List<BidModel> Bids { get; set; }

        public bool CanUserEdit { get; set; }
    }
}