using System;
using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Events
{
    public class EditEventViewModel
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public DateTime StartOn { get; set; }
        public DateTime EndOn { get; set; }
    }
}