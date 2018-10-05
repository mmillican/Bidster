using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidster.Models.EmailTemplates
{
    public class OutbidNoticeModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }

        public DateTime EventEndDate { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string UserName { get; set; }

        public DateTime OutbidTime { get; set; }

        public decimal OutbidAmount { get; set; }
        public decimal NewHighBidAmount { get; set; }
    }
}
