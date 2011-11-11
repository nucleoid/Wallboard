using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using Wallboard.Models;
using Wallboard.Tasks;

namespace Wallboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRssTasks _rssTasks;
        private readonly int _maxResults;

        public HomeController(IRssTasks rssTasks)
        {
            _rssTasks = rssTasks;
            _maxResults = Convert.ToInt32(ConfigurationManager.AppSettings["maxResults"]);
        }

        public ActionResult Index()
        {
            var buildStatusItems = _rssTasks.LoadItems(XmlReader.Create(ConfigurationManager.AppSettings["buildStatusesUrl"])).Take(_maxResults);
            var buildStatses = Mapper.Map<IEnumerable<SyndicationItem>, IEnumerable<BuildStatusModel>>(buildStatusItems);
            var repoCommits = _rssTasks.LoadItems(XmlReader.Create(ConfigurationManager.AppSettings["repositoryCommitsUrl"])).Take(_maxResults);
            var jiraIssues = _rssTasks.LoadItems(XmlReader.Create(ConfigurationManager.AppSettings["jiraUrl"])).Take(_maxResults);
            var model = new WallboardModel { BuildStatuses = buildStatses, RepositoryCommits = repoCommits, JiraIssues = jiraIssues };
            return View(model);
        }
    }
}
