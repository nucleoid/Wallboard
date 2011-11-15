using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using DDay.iCal;
using Wallboard.Models;
using Wallboard.Tasks;

namespace Wallboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRssTasks _rssTasks;
        private readonly ICalendarTasks _calendarTasks;
        private readonly IBambooTasks _bambooTasks;

        public HomeController(IRssTasks rssTasks, ICalendarTasks calendarTasks, IBambooTasks bambooTasks)
        {
            _rssTasks = rssTasks;
            _calendarTasks = calendarTasks;
            _bambooTasks = bambooTasks;
        }

        public ActionResult Index()
        {
            var model = GenerateWallboardModel();
            return View(model);
        }

        private WallboardModel GenerateWallboardModel()
        {
            var buildStatusItems = _bambooTasks.ProjectStatuses();
            var buildStatses = Mapper.Map<IEnumerable<string>, IEnumerable<BuildStatusModel>>(buildStatusItems);
            var holiday = _calendarTasks.LoadEvents(iCalendar.LoadFromUri(new Uri(ConfigurationManager.AppSettings["holidaysIcalUrl"])), DateTime.Now, null)
                .FirstOrDefault();
            var events = _calendarTasks.LoadEvents(iCalendar.LoadFromUri(new Uri(ConfigurationManager.AppSettings["eventsIcalUrl"])), DateTime.Now, null)
                .Take(8);
            var mappedEvents = Mapper.Map<IEnumerable<IEvent>, IEnumerable<EventModel>>(events);

            var model = new WallboardModel
            {
                BuildStatuses = buildStatses, 
                Holiday = holiday, 
                Events = mappedEvents
            };
            return model;
        }
    }
}
