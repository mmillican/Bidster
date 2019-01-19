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

            public DateTime BidTimestamp { get; set; }
            public decimal BidAmount { get; set; }

            public UserModel Winner { get; set; }

            public class UserModel
            {
                public int Id { get; set; }
                
                public string FullName { get; set; }
                public string Address { get; set; }
                public string Address2 { get; set; }
                public string City { get; set; }
                public string State { get; set; }
                public string PostalCode { get; set; }

                public string PhoneNumber { get; set; }
            }
        }
    }
}
