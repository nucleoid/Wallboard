
using System.Collections.Generic;

namespace Wallboard.Tasks.JsonSerialize
{
    public class BambooProjectsCollection
    {
        public string Expand { get; set; }
        public BambooLink Link { get; set; }
        public BambooProjects Projects { get; set; }
    }

    public class BambooLink
    {
        public string Href { get; set; }
        public string Rel { get; set; }
    }

    public class BambooProjects
    {
        public int StartIndex { get; set; }
        public int MaxResult { get; set; }
        public string Expand { get; set; }
        public IEnumerable<BambooProject> Project { get; set; }
    }

    public class BambooProject
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public BambooLink Link { get; set; }
    }
}