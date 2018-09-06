using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Models.Bids;

namespace Bidster.Models.Events
{
    public class AllBidsReportViewModel
    {
        public EventModel Event { get; set; }

        public List<ProductGroupModel> Products { get; set; } = new List<ProductGroupModel>();

        public class ProductGroupModel
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<BidModel> Bids { get; set; }
        }

    }
}
