using System.Configuration;
using System.Linq;
using MbUnit.Framework;
using RestSharp;
using Rhino.Mocks;
using Wallboard.Tasks;

namespace Wallboard.Tests.Tasks
{
    [TestFixture]
    public class BambooTasksTest
    {
        private IRestClient _restClient;
        private BambooTasks _tasks;

        [SetUp]
        public void Setup()
        {
            _restClient = MockRepository.GenerateMock<IRestClient>();
            _restClient.Expect(x => x.BaseUrl = ConfigurationManager.AppSettings["buildServerRestApi"]);
            _tasks = new BambooTasks(_restClient);
        }

        [Test]
        public void AllProjectKeysAndNames_Requests_Project_List()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project.xml")))
                .Return(new RestResponse { Content = ProjectsXml });

            //Act
            var projects = _tasks.AllProjectKeysAndNames();

            //Assert
            Assert.AreEqual(2, projects.Count());
            Assert.AreEqual("United Sugars - Procurement Portal", projects["CLUSCPP"]);
            Assert.AreEqual("United Sugars - Procurement Portal 2", projects["CLUSCPP2"]);
            _restClient.VerifyAllExpectations();

        }

        [Test]
        public void AllProjectKeysAndNames_Requests_Project_List_No_Projects()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project.xml")))
                .Return(new RestResponse { Content = NoProjectsXml });

            //Act
            var projects = _tasks.AllProjectKeysAndNames();

            //Assert
            Assert.AreEqual(0, projects.Count());
            _restClient.VerifyAllExpectations();

        }

        [Test]
        public void ProjectIsGreen_Failure()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.xml")))
                .Return(new RestResponse { Content = FailedProjectResults });

            //Act
            var result = _tasks.ProjectIsGreen("CLUSCPP");

            //Assert
            Assert.IsFalse(result);
            _restClient.VerifyAllExpectations();

        }

        [Test]
        public void ProjectIsGreen_Success()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.xml")))
                .Return(new RestResponse { Content = SuccessfulProjectResults });

            //Act
            var result = _tasks.ProjectIsGreen("CLUSCPP");

            //Assert
            Assert.IsTrue(result);
            _restClient.VerifyAllExpectations();

        }

        [Test]
        public void FailedPlanDetails_Requests_Failed_Plan_Details()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.xml")))
                .Return(new RestResponse { Content = FailedProjectResults });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP-DEV-4.xml")))
                .Return(new RestResponse { Content = FailedBuildDev4 });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP-QA-1.xml")))
                .Return(new RestResponse { Content = FailedBuildQA1 });

            //Act
            var details = _tasks.FailedPlanDetails("CLUSCPP");

            //Assert
            Assert.AreEqual(2, details.Count());
            Assert.Contains(details, "CLUSCPP-DEV-4 has FAILED : Manual build by <a href=\"http://tools.sycorr.com/build/browse/user/mpool\">Max Pool</a>");
            Assert.Contains(details, "CLUSCPP-QA-1 has FAILED : Initial clean build");
            _restClient.VerifyAllExpectations();
        }

        [Test]
        public void ProjectStatuses_Generates_All_Plan_Details()
        {
            //Arrange
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "project.xml")))
                .Return(new RestResponse { Content = ProjectsXml });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP.xml")))
                .Return(new RestResponse { Content = FailedProjectResults }).Repeat.Twice();
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP2.xml")))
                .Return(new RestResponse { Content = SuccessfulProjectResults });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP-DEV-4.xml")))
                .Return(new RestResponse { Content = FailedBuildDev4 });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP-QA-1.xml")))
                .Return(new RestResponse { Content = FailedBuildQA1 });

            //Act
            var details = _tasks.ProjectStatuses();

            //Assert
            Assert.AreEqual(3, details.Count());
            Assert.Contains(details, "Project United Sugars - Procurement Portal 2 has succeeded");
            Assert.Contains(details, "CLUSCPP-DEV-4 has FAILED : Manual build by <a href=\"http://tools.sycorr.com/build/browse/user/mpool\">Max Pool</a>");
            Assert.Contains(details, "CLUSCPP-QA-1 has FAILED : Initial clean build");
            _restClient.VerifyAllExpectations();
        }

        private string ProjectsXml
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><projects expand=\"projects\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/project\"/><projects expand=\"project\" size=\"2\" max-result=\"2\" start-index=\"0\"><project name=\"United Sugars - Procurement Portal\" key=\"CLUSCPP\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/project/CLUSCPP\"/></project><project name=\"United Sugars - Procurement Portal 2\" key=\"CLUSCPP2\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/project/CLUSCPP2\"/></project></projects></projects>";
            }
        }

        private string NoProjectsXml
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><projects expand=\"projects\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/project\"/><projects expand=\"project\" size=\"0\" max-result=\"0\" start-index=\"0\"></projects></projects>";
            }
        }

        private string SuccessfulProjectResults
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><results expand=\"results\"><results expand=\"result\" size=\"2\" max-result=\"2\" start-index=\"0\"><result id=\"688135\" number=\"4\" lifeCycleState=\"Finished\" state=\"Successful\" key=\"CLUSCPP-DEV-4\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\"/></result><result id=\"688137\" number=\"1\" lifeCycleState=\"Finished\" state=\"Successful\" key=\"CLUSCPP-QA-1\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\"/></result></results><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP\"/></results>";
            }
        }

        private string FailedProjectResults
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><results expand=\"results\"><results expand=\"result\" size=\"2\" max-result=\"2\" start-index=\"0\"><result id=\"688135\" number=\"4\" lifeCycleState=\"Finished\" state=\"Failed\" key=\"CLUSCPP-DEV-4\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\"/></result><result id=\"688137\" number=\"1\" lifeCycleState=\"Finished\" state=\"Failed\" key=\"CLUSCPP-QA-1\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\"/></result></results><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP\"/></results>";
            }
        }

        private string FailedBuildDev4
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><result restartable=\"true\" continuable=\"false\" id=\"688135\" number=\"4\" lifeCycleState=\"Finished\" state=\"Failed\" key=\"CLUSCPP-DEV-4\" expand=\"changes,metadata,vcsRevisions,artifacts,comments,labels,jiraIssues,stages\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\"/><planName>Development</planName><projectName>United Sugars - Procurement Portal</projectName><buildStartedTime>2011-11-12T11:05:13.616-06:00</buildStartedTime><prettyBuildStartedTime>Sat, 12 Nov, 11:05 AM</prettyBuildStartedTime><buildCompletedTime>2011-11-12T11:05:15.913-06:00</buildCompletedTime><prettyBuildCompletedTime>Sat, 12 Nov, 11:05 AM</prettyBuildCompletedTime><buildDurationInSeconds>2</buildDurationInSeconds><buildDuration>2297</buildDuration><buildDurationDescription>2 seconds</buildDurationDescription><buildRelativeTime>2 days ago</buildRelativeTime><vcsRevisionKey>49</vcsRevisionKey><vcsRevisions size=\"1\" max-result=\"1\" start-index=\"0\"/><buildTestSummary>No tests found</buildTestSummary><successfulTestCount>0</successfulTestCount><failedTestCount>0</failedTestCount><buildReason>Manual build by &lt;a href=&quot;http://tools.sycorr.com/build/browse/user/mpool&quot;&gt;Max Pool&lt;/a&gt;</buildReason><artifacts size=\"0\" max-result=\"0\" start-index=\"0\"/><comments size=\"0\" max-result=\"0\" start-index=\"0\"/><labels size=\"0\" max-result=\"0\" start-index=\"0\"/><jiraIssues size=\"0\" max-result=\"0\" start-index=\"0\"/><stages size=\"1\" max-result=\"1\" start-index=\"0\"/><changes size=\"0\" max-result=\"0\" start-index=\"0\"/><metadata size=\"2\" max-result=\"2\" start-index=\"0\"/></result>";
            }
        }

        private string FailedBuildQA1
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><result restartable=\"true\" continuable=\"false\" id=\"688137\" number=\"1\" lifeCycleState=\"Finished\" state=\"Failed\" key=\"CLUSCPP-QA-1\" expand=\"changes,metadata,vcsRevisions,artifacts,comments,labels,jiraIssues,stages\"><link rel=\"self\" href=\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\"/><planName>QA</planName><projectName>United Sugars - Procurement Portal</projectName><buildStartedTime>2011-11-12T11:12:48.500-06:00</buildStartedTime><prettyBuildStartedTime>Sat, 12 Nov, 11:12 AM</prettyBuildStartedTime><buildCompletedTime>2011-11-12T11:14:45.319-06:00</buildCompletedTime><prettyBuildCompletedTime>Sat, 12 Nov, 11:14 AM</prettyBuildCompletedTime><buildDurationInSeconds>116</buildDurationInSeconds><buildDuration>116819</buildDuration><buildDurationDescription>1 minute</buildDurationDescription><buildRelativeTime>2 days ago</buildRelativeTime><vcsRevisionKey>49</vcsRevisionKey><vcsRevisions size=\"1\" max-result=\"1\" start-index=\"0\"/><buildTestSummary>No tests found</buildTestSummary><successfulTestCount>0</successfulTestCount><failedTestCount>0</failedTestCount><buildReason>Initial clean build</buildReason><artifacts size=\"0\" max-result=\"0\" start-index=\"0\"/><comments size=\"0\" max-result=\"0\" start-index=\"0\"/><labels size=\"0\" max-result=\"0\" start-index=\"0\"/><jiraIssues size=\"0\" max-result=\"0\" start-index=\"0\"/><stages size=\"1\" max-result=\"1\" start-index=\"0\"/><changes size=\"0\" max-result=\"0\" start-index=\"0\"/><metadata size=\"0\" max-result=\"0\" start-index=\"0\"/></result>";
            }
        }
    }
}
