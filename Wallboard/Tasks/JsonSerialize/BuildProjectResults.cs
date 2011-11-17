
namespace Wallboard.Tasks.JsonSerialize
{
    public class BuildProjectResults
    {
        public string Expand { get; set; }
        public BuildProjectLink Link { get; set; }
        public string PlanName { get; set; }
        public string ProjectName { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public string LifeCycleState { get; set; }
        public int Number { get; set; }
        public int Id { get; set; }
        public string BuildStartedTime { get; set; }
        public string PrettyBuildStartedTime { get; set; }
        public int BuildDurationInSeconds { get; set; }
        public int BuildDuration { get; set; }
        public string BuildDurationDescription { get; set; }
        public string BuildRelativeTime { get; set; }
        public string VcsRevisionKey { get; set; }
        public CollectionElement VcsRevisions { get; set; }
        public string BuildTestSummary { get; set; }
        public int SuccessfulTestCount { get; set; }
        public int FailedTestCount { get; set; }
        public bool Continuable { get; set; }
        public bool Restartable { get; set; }
        public string BuildReason { get; set; }
        public CollectionElement Artifacts { get; set; }
        public CollectionElement Comments { get; set; }
        public CollectionElement Labels { get; set; }
        public CollectionElement JiraIssues { get; set; }
        public CollectionElement Stages { get; set; }
        public CollectionElement Changes { get; set; }
        public CollectionElement Metadata { get; set; }
    }

    public class BuildProjectLink
    {
        public string Href { get; set; }
        public string Rel { get; set; }
    }

    public class CollectionElement
    {
        public int StartIndex { get; set; }
        public int MaxResult { get; set; }
        public int Size { get; set; }
    }
}