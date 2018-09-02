using System;

namespace Bidster.Models.Bids
{
    public class BidModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Amount { get; set; }
    }
}