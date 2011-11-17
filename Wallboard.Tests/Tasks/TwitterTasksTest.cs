using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using MbUnit.Framework;
using Wallboard.Tasks;

namespace Wallboard.Tests.Tasks
{
    [TestFixture]
    public class TwitterTasksTest
    {
        private TwitterTasks _tasks;

        [SetUp]
        public void Setup()
        {
            _tasks = new TwitterTasks();
        }

        [Test]
        public void FilterAndSortTweets_Filters_And_Sorts()
        {
            //Arrange
            var date = DateTime.Parse("11/16/2011");
            var queryable = FilterSortNeededQueryable(date);

            //Act
            var queried = _tasks.FilterAndSortTweets(queryable, 10, "mstatz");

            //Assert
            Assert.AreEqual(10, queried.Count);
            Assert.Sorted(queried, SortOrder.Decreasing, new CompareStatuses());
            Assert.ForAll(queried, x => x.Type == StatusType.User);
            Assert.ForAll(queried, x => x.ExcludeReplies);
            Assert.ForAll(queried, x => x.IncludeRetweets);
            Assert.ForAll(queried, x => x.ScreenName == "mstatz");
        }

        [Test]
        public void FilterAndSortTweets_Handles_Multiple_Usernames()
        {
            //Arrange
            var date = DateTime.Parse("11/16/2011");
            var queryable = MultipleUserNameQueryable(date);

            //Act
            var queried = _tasks.FilterAndSortTweets(queryable, 10, "mstatz", "nodakpaul");

            //Assert
            Assert.AreEqual(4, queried.Count);
            Assert.ForAll(queried, x => x.ScreenName == "mstatz" || x.ScreenName == "nodakpaul");
        }

        [Test]
        [Row(TwitterImageSize.Mini, "mini", "mstatz")]
        [Row(TwitterImageSize.Mini, "mini", "sycorr")]
        [Row(TwitterImageSize.Bigger, "bigger", "mstatz")]
        [Row(TwitterImageSize.Normal, "normal", "mstatz")]
        [Row(TwitterImageSize.Original, "original", "mstatz")]
        public void ProfileImageUrl_Generates_CorrectUrl(TwitterImageSize size, string realSize, string username)
        {
            //Act
            var url = _tasks.ProfileImageUrl(username, size);

            //Assert
            Assert.AreEqual(string.Format("https://api.twitter.com/1/users/profile_image?screen_name={0}&size={1}", username, realSize), url);
        }

        private class CompareStatuses : IComparer<Status>
        {
            public int Compare(Status x, Status y)
            {
                return x.CreatedAt.CompareTo(y.CreatedAt);
            }
        }

        //http://api.twitter.com/1/statuses/user_timeline.atom?screen_name=mpool&include_rts=true&count=10&exclude_replies=true
        private IQueryable<Status> FilterSortNeededQueryable(DateTime date)
        {
            return new List<Status>
                       {
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 9, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date.AddDays(-5), Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date.AddDays(-2), Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.Friends, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date,  Type = StatusType.Mentions, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = false, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = false},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = false, IncludeRetweets = false},
                           new Status {ScreenName = "sycorr", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "sycorr", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                       }.AsQueryable();
        }

        private IQueryable<Status> MultipleUserNameQueryable(DateTime date)
        {
            return new List<Status>
                       {
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "sycorr", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "nodakpaul", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "nodakpaul", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "sycorr", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                           new Status {ScreenName = "mstatz", CreatedAt = date, Type = StatusType.User, Count = 10, ExcludeReplies = true, IncludeRetweets = true},
                       }.AsQueryable();
        }
    }
}
