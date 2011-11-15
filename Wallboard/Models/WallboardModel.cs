using System.Collections.Generic;
using DDay.iCal;

namespace Wallboard.Models
{
    public class WallboardModel
    {
        public IEvent Holiday { get; set; }
        public IEnumerable<EventModel> Events { get; set; }
        public IEnumerable<BuildStatusModel> BuildStatuses { get; set; }
    }
}