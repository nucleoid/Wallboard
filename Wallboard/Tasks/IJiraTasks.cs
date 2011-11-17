using System.Collections.Generic;

namespace Wallboard.Tasks
{
    public interface IJiraTasks
    {
        Dictionary<string, string> AllProjectKeysAndNames();
    }
}