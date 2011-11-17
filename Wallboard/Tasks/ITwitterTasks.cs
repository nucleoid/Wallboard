
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;

namespace Wallboard.Tasks
{
    public interface ITwitterTasks
    {
        IList<Status> FilterAndSortTweets(int count, params string[] usernames);
        IList<Status> FilterAndSortTweets(IQueryable<Status> queryable, int count, params string[] usernames);
        string ProfileImageUrl(string username, TwitterImageSize size);
    }
}