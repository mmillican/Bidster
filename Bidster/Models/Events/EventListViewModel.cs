using System.Collections.Generic;

namespace Bidster.Models.Events
{
    public class EventListViewModel
    {
        public List<EventModel> Events { get; set; }
        public bool CanCreateEvent { get; set; }
    }
}