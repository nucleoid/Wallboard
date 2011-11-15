
using System;
using System.Collections.Generic;
using DDay.iCal;

namespace Wallboard.Tasks
{
    public interface ICalendarTasks
    {
        IEnumerable<IEvent> LoadEvents(IICalendarCollection calendars, DateTime start, DateTime? end);
    }
}