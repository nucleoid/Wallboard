﻿using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Wallboard.Tasks
{
    public interface IRssTasks
    {
        IEnumerable<SyndicationItem> LoadAndSortItems(params XmlReader[] readers);
    }
}