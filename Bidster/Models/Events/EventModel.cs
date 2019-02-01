using System;

namespace Bidster.Models.Events
{
    public class EventModel
    {
        public int Id { get; set; }

        public string Slug { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartOn { get; set; }
        public DateTime EndOn { get; set; }
        public DateTime DisplayOn { get; set; }

        public int OwnerId { get; set; }
        public bool HideBidderNames { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsBiddingOpen { get; set; }
    }
}