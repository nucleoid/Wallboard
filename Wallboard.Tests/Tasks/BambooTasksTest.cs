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
            _tasks = new BambooTasks();
        }

        [Test]
        public void Constructor_Sets_BaseUrl()
        {
            //Assert
            Assert.AreEqual(ConfigurationManager.AppSettings["buildServerRestApi"], (_tasks.GetField("_restClient") as IRestClient).BaseUrl);
        }

        [Test]
        public void AllProjectKeysAndNames_Requests_Project_List()
        {
            //Arrange
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project.json")))
                .Return(new RestResponse { Content = ProjectsJson });

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
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project.json")))
                .Return(new RestResponse { Content = NoProjectsJson });

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
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.json")))
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
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.json")))
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
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP.json")))
                .Return(new RestResponse { Content = FailedProjectResults });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP-DEV-4.json")))
                .Return(new RestResponse { Content = FailedBuildDev4 });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "result/CLUSCPP-QA-1.json")))
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
            _tasks.SetField<BambooTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "project.json")))
                .Return(new RestResponse { Content = ProjectsJson });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP.json")))
                .Return(new RestResponse { Content = FailedProjectResults }).Repeat.Twice();
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP2.json")))
                .Return(new RestResponse { Content = SuccessfulProjectResults });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP-DEV-4.json")))
                .Return(new RestResponse { Content = FailedBuildDev4 });
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Resource == "result/CLUSCPP-QA-1.json")))
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

        private string ProjectsJson
        {
            get
            {
                return "{\"expand\":\"projects\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/project\",\"rel\":\"self\"},\"projects\":{\"start-index\":0,\"max-result\":1,\"size\":1,\"expand\":\"project\",\"project\":[{\"key\":\"CLUSCPP\",\"name\":\"United Sugars - Procurement Portal\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/project/CLUSCPP\",\"rel\":\"self\"}},{\"key\":\"CLUSCPP2\",\"name\":\"United Sugars - Procurement Portal 2\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/project/CLUSCPP2\",\"rel\":\"self\"}}]}}";
            }
        }

        private string NoProjectsJson
        {
            get { return "[]"; }
        }

        private string SuccessfulProjectResults
        {
            get
            {
                return "{\"results\":{\"start-index\":0,\"max-result\":2,\"size\":2,\"result\":[{\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\",\"rel\":\"self\"},\"key\":\"CLUSCPP-DEV-4\",\"state\":\"Successful\",\"lifeCycleState\":\"Finished\",\"number\":4,\"id\":688179},{\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\",\"rel\":\"self\"},\"key\":\"CLUSCPP-QA-1\",\"state\":\"Successful\",\"lifeCycleState\":\"Finished\",\"number\":11,\"id\":688177}],\"expand\":\"result\"},\"expand\":\"results\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP\",\"rel\":\"self\"}}";
            }
        }

        private string FailedProjectResults
        {
            get
            {
                return "{\"results\":{\"start-index\":0,\"max-result\":2,\"size\":2,\"result\":[{\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\",\"rel\":\"self\"},\"key\":\"CLUSCPP-DEV-4\",\"state\":\"Failed\",\"lifeCycleState\":\"Finished\",\"number\":4,\"id\":688179},{\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\",\"rel\":\"self\"},\"key\":\"CLUSCPP-QA-1\",\"state\":\"Failed\",\"lifeCycleState\":\"Finished\",\"number\":11,\"id\":688177}],\"expand\":\"result\"},\"expand\":\"results\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP\",\"rel\":\"self\"}}";
            }
        }

        private string FailedBuildDev4
        {
            get
            {
                return "{\"expand\":\"changes,metadata,vcsRevisions,artifacts,comments,labels,jiraIssues,stages\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-DEV-4\",\"rel\":\"self\"},\"planName\":\"Development\",\"projectName\":\"United Sugars - Procurement Portal\",\"key\":\"CLUSCPP-DEV-4\",\"state\":\"Failed\",\"lifeCycleState\":\"Finished\",\"number\":4,\"id\":688135,\"buildStartedTime\":\"2011-11-12T11:05:13.616-06:00\",\"prettyBuildStartedTime\":\"Sat, 12 Nov, 11:05 AM\",\"buildCompletedTime\":\"2011-11-12T11:05:4.913-06:00\",\"prettyBuildCompletedTime\":\"Sat, 12 Nov, 11:05 AM\",\"buildDurationInSeconds\":2,\"buildDuration\":2297,\"buildDurationDescription\":\"2 seconds\",\"buildRelativeTime\":\"4 days ago\",\"vcsRevisionKey\":\"49\",\"vcsRevisions\":{\"start-index\":0,\"max-result\":1,\"size\":1},\"buildTestSummary\":\"No tests found\",\"successfulTestCount\":0,\"failedTestCount\":0,\"continuable\":false,\"restartable\":true,\"buildReason\":\"Manual build by <a href=\\\"http://tools.sycorr.com/build/browse/user/mpool\\\">Max Pool</a>\",\"artifacts\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"comments\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"labels\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"jiraIssues\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"stages\":{\"start-index\":0,\"max-result\":1,\"size\":1},\"changes\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"metadata\":{\"start-index\":0,\"max-result\":2,\"size\":2}}";
            }
        }

        private string FailedBuildQA1
        {
            get
            {
                return "{\"expand\":\"changes,metadata,vcsRevisions,artifacts,comments,labels,jiraIssues,stages\",\"link\":{\"href\":\"http://tools.sycorr.com/build/rest/api/latest/result/CLUSCPP-QA-1\",\"rel\":\"self\"},\"planName\":\"QA\",\"projectName\":\"United Sugars - Procurement Portal\",\"key\":\"CLUSCPP-QA-1\",\"state\":\"Failed\",\"lifeCycleState\":\"Finished\",\"number\":1,\"id\":688137,\"buildStartedTime\":\"2011-11-12T11:12:48.500-06:00\",\"prettyBuildStartedTime\":\"Sat, 12 Nov, 11:12 AM\",\"buildCompletedTime\":\"2011-11-12T11:14:45.319-06:00\",\"prettyBuildCompletedTime\":\"Sat, 12 Nov, 11:14 AM\",\"buildDurationInSeconds\":116,\"buildDuration\":116819,\"buildDurationDescription\":\"1 minute\",\"buildRelativeTime\":\"4 days ago\",\"vcsRevisionKey\":\"49\",\"vcsRevisions\":{\"start-index\":0,\"max-result\":1,\"size\":1},\"buildTestSummary\":\"No tests found\",\"successfulTestCount\":0,\"failedTestCount\":0,\"continuable\":false,\"restartable\":true,\"buildReason\":\"Initial clean build\",\"artifacts\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"comments\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"labels\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"jiraIssues\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"stages\":{\"start-index\":0,\"max-result\":1,\"size\":1},\"changes\":{\"start-index\":0,\"max-result\":0,\"size\":0},\"metadata\":{\"start-index\":0,\"max-result\":0,\"size\":0}}";
            }
        }
    }
}
