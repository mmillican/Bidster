using System;
using System.ComponentModel.DataAnnotations.Schema;
using Bidster.Entities.Users;

namespace Bidster.Entities.Events
{
    public class EventUser
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
