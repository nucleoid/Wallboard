using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Wallboard.Tasks
{
    public class RssTasks : IRssTasks
    {
        public IEnumerable<SyndicationItem> LoadItems(params XmlReader[] readers)
        {
            var items = new List<SyndicationItem>();
            foreach (var xmlReader in readers)
                items.AddRange(SyndicationFeed.Load(xmlReader).Items);

            return items.Distinct(new ItemComparer()).OrderByDescending(x => x.PublishDate);
        }

        private class ItemComparer : IEqualityComparer<SyndicationItem>
        {
            public bool Equals(SyndicationItem x, SyndicationItem y)
            {
                return x.Title.Text == y.Title.Text;
            }

            public int GetHashCode(SyndicationItem obj)
            {
                return obj.Title.Text.GetHashCode();
            }
        }
    }
}