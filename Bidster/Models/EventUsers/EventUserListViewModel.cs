using System.Collections.Generic;
using Bidster.Models.Events;

namespace Bidster.Models.EventUsers
{
    public class EventUserListViewModel
    {
        public EventModel Event { get; set; }

        public List<EventUserModel> Users { get; set; }

        public string NewUserEmail { get; set; }
        public bool NewUserIsAdmin { get; set; }
    }
}
