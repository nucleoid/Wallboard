using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using MbUnit.Framework;
using Wallboard.Tasks;

namespace Wallboard.Tests.Tasks
{
    [TestFixture]
    public class RssTasksTest
    {
        private IRssTasks _tasks;

        [SetUp]
        public void Setup()
        {
            _tasks = new RssTasks();
        }

        [Test]
        public void LoadItems_Reader_Sorts_And_Excludes_Duplicate_Items()
        {
            //Arrange
            var reader = XmlReader.Create(new StringReader(TestRssFeed));

            //Act
            var items = _tasks.LoadItems(reader).ToList();

            Assert.AreEqual(2, items.Count);
            Assert.Sorted(items, SortOrder.Decreasing, new CompareItems());
            Assert.AreEqual("CLUSCPP-DEV-2 has FAILED : Updated by James Ehly", items[0].Title.Text);
            Assert.AreEqual("CLUSCPP-DEV-1 has FAILED : Initial clean build", items[1].Title.Text);
        }

        [Test]
        public void LoadItems_Reader_Loads_Multiple_Feeds()
        {
            //Arrange
            var reader = XmlReader.Create(new StringReader(TestRssFeed));
            var reader2 = XmlReader.Create(new StringReader(TestRssFeed2));

            //Act
            var items = _tasks.LoadItems(reader, reader2);

            Assert.AreEqual(4, items.Count());
            Assert.Sorted(items, SortOrder.Decreasing, new CompareItems());
        }

        private class CompareItems : IComparer<SyndicationItem>
        {
            public int Compare(SyndicationItem x, SyndicationItem y)
            {
                return x.PublishDate.CompareTo(y.PublishDate);
            }
        }

        private string TestRssFeed
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss xmlns:dc=\"http://purl.org/dc/elements/1.1/\" version=\"2.0\"><channel>" +
                       "<title>Bamboo build results feed for all builds</title><link>http://tools.sycorr.com/build</link><description>" +
                       "This feed is updated whenever a build gets built</description><item><title>" +
                       "CLUSCPP-DEV-1 has FAILED : Initial clean build</title><link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</link>" +
                       "<description>&lt;p&gt;This is the initial clean build.&lt;/p&gt;&lt;p&gt;The build has 0 failed tests and 0 successful tests." +
                       "&lt;/p&gt;</description><pubDate>Fri, 04 Nov 2011 03:28:28 GMT</pubDate><guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</guid>" +
                       "<dc:date>2011-11-04T03:28:28Z</dc:date></item><item><title>" +
                       "CLUSCPP-DEV-1 has FAILED : Initial clean build</title><link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</link>" +
                       "<description>&lt;p&gt;This is the initial clean build.&lt;/p&gt;&lt;p&gt;The build has 0 failed tests and 0 successful tests." +
                       "&lt;/p&gt;</description><pubDate>Fri, 04 Nov 2011 03:28:28 GMT</pubDate><guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</guid>" +
                       "<dc:date>2011-11-04T03:28:28Z</dc:date></item><item><title>CLUSCPP-DEV-2 has FAILED : Updated by James Ehly</title>" +
                       "<link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-2</link><description>&lt;p&gt;Code has been updated by &lt;a " +
                       "href=\"http://tools.sycorr.com/build/browse/user/jehly\"&gt;James Ehly&lt;/a&gt;.&lt;/p&gt;&lt;p&gt;CLUSCPP-DEV-2 has the following " +
                       "1 changes:&lt;/p&gt;&lt;p&gt;James Ehly made the following changes at 09 Nov 2011, 11:39:51 PM&lt;br&gt;with the comment: " +
                       "Balloons: only show if clicked, add down arrow, and fix intents. Change head click event to double click&lt;/p&gt;&lt;ul&gt;" +
                       "&lt;li&gt;/trunk/src/USC.ProcurementPortal.Sales.Web/assets/js/common.js&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/Views/Pricing/Analysis.aspx&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/assets/css/main.css&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/assets/images/template/tooltip_arrow_down.png&lt;/li&gt;&lt;/ul&gt;&lt;p&gt;" +
                       "The build has 0 failed tests and 0 successful tests.&lt;/p&gt;</description><pubDate>Thu, 10 Nov 2011 05:40:23 GMT</pubDate>" +
                       "<guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-2</guid><dc:date>2011-11-10T05:40:23Z</dc:date></item></channel></rss>";
            }
        }

        private string TestRssFeed2
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><rss xmlns:dc=\"http://purl.org/dc/elements/1.1/\" version=\"2.0\"><channel>" +
                       "<title>Bamboo build results feed for all builds</title><link>http://tools.sycorr.com/build</link><description>" +
                       "This feed is updated whenever a build gets built</description><item><title>" +
                       "blah CLUSCPP-DEV-1 has FAILED : Initial clean build</title><link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</link>" +
                       "<description>&lt;p&gt;This is the initial clean build.&lt;/p&gt;&lt;p&gt;The build has 0 failed tests and 0 successful tests." +
                       "&lt;/p&gt;</description><pubDate>Fri, 04 Nov 2011 03:28:28 GMT</pubDate><guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</guid>" +
                       "<dc:date>2011-11-04T03:28:28Z</dc:date></item><item><title>" +
                       "blah CLUSCPP-DEV-1 has FAILED : Initial clean build</title><link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</link>" +
                       "<description>&lt;p&gt;This is the initial clean build.&lt;/p&gt;&lt;p&gt;The build has 0 failed tests and 0 successful tests." +
                       "&lt;/p&gt;</description><pubDate>Fri, 04 Nov 2011 03:28:28 GMT</pubDate><guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-1</guid>" +
                       "<dc:date>2011-11-04T03:28:28Z</dc:date></item><item><title>CLUSCPP-DEV-2 has FAILED : Updated by James Ehly 2</title>" +
                       "<link>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-2</link><description>&lt;p&gt;Code has been updated by &lt;a " +
                       "href=\"http://tools.sycorr.com/build/browse/user/jehly\"&gt;James Ehly&lt;/a&gt;.&lt;/p&gt;&lt;p&gt;CLUSCPP-DEV-2 has the following " +
                       "1 changes:&lt;/p&gt;&lt;p&gt;James Ehly made the following changes at 09 Nov 2011, 11:39:51 PM&lt;br&gt;with the comment: " +
                       "Balloons: only show if clicked, add down arrow, and fix intents. Change head click event to double click&lt;/p&gt;&lt;ul&gt;" +
                       "&lt;li&gt;/trunk/src/USC.ProcurementPortal.Sales.Web/assets/js/common.js&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/Views/Pricing/Analysis.aspx&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/assets/css/main.css&lt;/li&gt;&lt;li&gt;" +
                       "/trunk/src/USC.ProcurementPortal.Sales.Web/assets/images/template/tooltip_arrow_down.png&lt;/li&gt;&lt;/ul&gt;&lt;p&gt;" +
                       "The build has 0 failed tests and 0 successful tests.&lt;/p&gt;</description><pubDate>Thu, 10 Nov 2011 05:40:23 GMT</pubDate>" +
                       "<guid>http://tools.sycorr.com/build/browse/CLUSCPP-DEV-2</guid><dc:date>2011-11-10T05:40:23Z</dc:date></item></channel></rss>";
            }
        }
    }
}
