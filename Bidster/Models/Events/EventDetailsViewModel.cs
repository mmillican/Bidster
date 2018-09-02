using System.Collections.Generic;
using Bidster.Models.Products;

namespace Bidster.Models.Events
{
    public class EventDetailsViewModel
    {
        public EventModel Event { get; set; }

        public List<ProductModel> Products { get; set; }
    }
}