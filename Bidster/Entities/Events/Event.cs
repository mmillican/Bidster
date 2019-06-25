using Bidster.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidster.Entities.Events
{
    public class Event 
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Slug { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartOn { get; set; }
        public DateTime EndOn { get; set; }

        public DateTime DisplayOn { get; set; }

        public bool HideBidderNames { get; set; }

        public decimal? DefaultMinimumBidAmount { get; set; }

        public int OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }

        public DateTime CreatedOn { get; set; }


        public bool IsBiddingOpen(DateTime date) => StartOn <= date && EndOn > date;

        public virtual List<EventUser> Users { get; set; }
    }
}