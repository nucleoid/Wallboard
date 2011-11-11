using System.ServiceModel.Syndication;

namespace Wallboard.Models
{
    public class BuildStatusModel
    {
        public SyndicationItem BuildStatus { get; set; }
        public string StatusClass { get; set; }
    }
}