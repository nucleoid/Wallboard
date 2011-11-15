﻿using System;
using System.Collections.Generic;
using System.Linq;
using DDay.iCal;

namespace Wallboard.Tasks
{
    public class CalendarTasks : ICalendarTasks
    {
        public IEnumerable<IEvent> LoadEvents(IICalendarCollection calendars, DateTime start, DateTime? end)
        {
            if (end.HasValue)
                return calendars[0].Events.Where(x => x.Start.Value >= start && x.End.Value <= end.Value).OrderBy(y => y.Start);
            return calendars[0].Events.Where(x => x.Start.Value >= start).OrderBy(y => y.Start);
        }
    }
}