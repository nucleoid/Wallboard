using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Wallboard.Tasks
{
    public class RssTasks : IRssTasks
    {
        public IEnumerable<SyndicationItem> LoadItems(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            return feed.Items;
        }
    }
}