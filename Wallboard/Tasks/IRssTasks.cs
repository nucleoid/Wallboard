using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Wallboard.Tasks
{
    public interface IRssTasks
    {
        /// <summary>
        /// Loads a collection of RSS items from a given feed.
        /// Easiest usage is to use XmlReader.Create(string feedUrl)
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A collection of RSS items from the </returns>
        IEnumerable<SyndicationItem> LoadItems(XmlReader reader);
    }
}