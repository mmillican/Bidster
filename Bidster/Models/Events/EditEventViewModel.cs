using System;
using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Events
{
    public class EditEventViewModel
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "Event description")]
        public string Description { get; set; }

        [Display(Name = "Start date/time")]
        public DateTime StartOn { get; set; }
        [Display(Name = "End date/time")]
        public DateTime EndOn { get; set; }
        [Display(Name = "Display date/time")]
        public DateTime DisplayOn { get; set; }

        [Display(Name = "Hide bidder names")]
        public bool HideBidderNames { get; set; }
        [Display(Name = "Default min bid amount")]
        public decimal? DefaultMinimumBidAmount { get; set; }
    }
}