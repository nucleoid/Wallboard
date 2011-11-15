
using System.Collections.Generic;

namespace Wallboard.Tasks
{
    public interface IBambooTasks
    {
        Dictionary<string, string> AllProjectKeysAndNames();
        bool ProjectIsGreen(string projectKey);
        IEnumerable<string> FailedPlanDetails(string projectKey);
        IEnumerable<string> ProjectStatuses();
    }
}