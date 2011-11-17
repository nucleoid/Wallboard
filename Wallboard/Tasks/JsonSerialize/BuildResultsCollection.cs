using System.Collections.Generic;

namespace Wallboard.Tasks.JsonSerialize
{
    public class BuildResultsCollection
    {
        public BuildResults Results { get; set; }
        public string Expand { get; set; }
        public BuildLink Link { get; set; }
    }

    public class BuildResults
    {
        public int StartIndex { get; set; }
        public int MaxResult { get; set; }
        public int Size { get; set; }
        public IEnumerable<BuildResult> Result { get; set; }
        public string Expand { get; set; }
    }

    public class BuildResult
    {
        public BuildLink Link { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public string LifeCycleState { get; set; }
        public int Number { get; set; }
        public int Id { get; set; }
    }

    public class BuildLink
    {
        public string Href { get; set; }
        public string Rel { get; set; }
    }
}