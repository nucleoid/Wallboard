using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using DDay.iCal;
using LinqToTwitter;
using Wallboard.Models;
using Wallboard.Tasks;

namespace Wallboard.Controllers
{
    public class FeedController : Controller
    {
        private readonly IRssTasks _rssTasks;
        private readonly ITwitterTasks _twitterTasks;
        private readonly IBambooTasks _bambooTasks;
        private readonly ICalendarTasks _calendarTasks;
        private readonly IJiraTasks _jiraTasks;

        public FeedController(IRssTasks rssTasks, ITwitterTasks twitterTasks, IBambooTasks bambooTasks, ICalendarTasks calendarTasks, IJiraTasks jiraTasks)
        {
            _rssTasks = rssTasks;
            _twitterTasks = twitterTasks;
            _bambooTasks = bambooTasks;
            _calendarTasks = calendarTasks;
            _jiraTasks = jiraTasks;
        }

        public ActionResult Rss()
        {
            var rssFeeds = ConfigurationManager.AppSettings["newsUrls"].Split('|');
            var feedReaders = rssFeeds.Select(XmlReader.Create).ToArray();
            var newsItems = _rssTasks.LoadAndSortItems(feedReaders);
            var newsTitles = newsItems.Select(x => x.Title.Text.Length > 100 ? string.Format("{0} ...", x.Title.Text.Substring(0, 100)) : x.Title.Text);
            return PartialView(newsTitles);
        }

        public ActionResult Twitter()
        {
            var twitterProfiles = ConfigurationManager.AppSettings["twitterscreennames"].Split('|');
            var twitterItems = _twitterTasks.FilterAndSortTweets(20, twitterProfiles);
            var twitterStatuses = twitterItems.Select(CombineProfileAndTweet);
            return PartialView("Rss", twitterStatuses);
        }

        public ActionResult BuildStatuses()
        {
            var buildStatusItems = _bambooTasks.ProjectStatuses();
            var buildStatses = Mapper.Map<IEnumerable<string>, IEnumerable<BuildStatusModel>>(buildStatusItems);
            return PartialView(buildStatses);
        }

        public ActionResult JiraProjects()
        {
            var projects = _jiraTasks.AllProjectKeysAndNames();
            return PartialView(projects);
        }

        public ActionResult Calendar()
        {
            var events = _calendarTasks.LoadEvents(iCalendar.LoadFromUri(new Uri(ConfigurationManager.AppSettings["eventsIcalUrl"])), DateTime.Now, null)
                .Take(8);
            var mappedEvents = Mapper.Map<IEnumerable<IEvent>, IEnumerable<EventModel>>(events);
            return PartialView(mappedEvents);
        }

        public ActionResult NextHoliday()
        {
            var holiday = _calendarTasks.LoadEvents(iCalendar.LoadFromUri(new Uri(ConfigurationManager.AppSettings["holidaysIcalUrl"])), DateTime.Now, null)
                .FirstOrDefault();
            return PartialView(holiday);
        }

        private string CombineProfileAndTweet(Status status)
        {
            var combined = string.Format("<img class='twitter-headline' src='{0}' /> {1} {2}", 
                _twitterTasks.ProfileImageUrl(status.ScreenName, TwitterImageSize.Mini), status.CreatedAt, status.Text);
            return combined;
        }
    }
}
