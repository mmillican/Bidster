using System;
using System.Collections.Generic;

namespace Bidster.Models.Events
{
    public class WinningBidsReportViewModel
    {
        public EventModel Event { get; set; }

        public List<BidRecordModel> WinningBids { get; set; } = new List<BidRecordModel>();

        public class BidRecordModel
        {
            public int Id { get; set; }

            public int ProductId { get; set; }
            public string ProductName { get; set; }

            public int UserId { get; set; }
            public string UserName { get; set; }

            public DateTime BidTimestamp { get; set; }
            public decimal BidAmount { get; set; }
        }
    }
}
