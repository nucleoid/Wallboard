
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinqToTwitter;

namespace Wallboard.Tasks
{
    public class TwitterTasks : ITwitterTasks
    {
        private const string ImageUrl = "https://api.twitter.com/1/users/profile_image?screen_name={0}&size={1}";

        public IList<Status> FilterAndSortTweets(int count, params string[] usernames)
        {
            var auth = new SingleUserAuthorizer
                           {
                               Credentials = new InMemoryCredentials
                                {
                                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
                                    OAuthToken = ConfigurationManager.AppSettings["twitterOAuthToken"],
                                    AccessToken = ConfigurationManager.AppSettings["twitterAccessToken"]
                                }
                           };
            return FilterAndSortTweets(new TwitterContext(auth).Status, count, usernames);
        }

        public IList<Status> FilterAndSortTweets(IQueryable<Status> queryable, int count, params string[] usernames)
        {
            var statuses = new List<Status>();
            foreach (var username in usernames)
                statuses.AddRange(queryable.Where(x => x.ScreenName == username && x.Type == StatusType.User && x.ExcludeReplies == true
                    && x.IncludeRetweets == true && x.Count == count).OrderByDescending(y => y.CreatedAt));

            return statuses.OrderByDescending(x => x.CreatedAt).Take(count).ToList();
        }

        public string ProfileImageUrl(string username, TwitterImageSize size)
        {
            return string.Format(ImageUrl, username, size.ToString().ToLowerInvariant());
        }
    }
}