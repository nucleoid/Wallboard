using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace Wallboard.Models
{
    public class WallboardModel
    {
        public IEnumerable<BuildStatusModel> BuildStatuses { get; set; }
        public IEnumerable<SyndicationItem> RepositoryCommits { get; set; }
        public IEnumerable<SyndicationItem> JiraIssues { get; set; } 
    }
}