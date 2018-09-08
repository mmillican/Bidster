using System;

namespace Bidster.Models.EventUsers
{
    public class EventUserModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
