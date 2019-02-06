using Bidster.Models.Events;
using System;
using System.Collections.Generic;

namespace Bidster.Models.Home
{
    public class HomeViewModel
    {
        public List<EventModel> Events { get; set; }

        public List<BidSummaryModel> UserBids { get; set; } = new List<BidSummaryModel>();
    }

    public class BidSummaryModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventSlug { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }

        public DateTime EventEndTime { get; set; }

        public decimal CurrentBidAmount { get; set; }
        public int HighBigUserId { get; set; }
        public bool IsWinning { get; set; }
    }
}
