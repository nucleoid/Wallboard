using System.Configuration;
using System.Net;
using MbUnit.Framework;
using RestSharp;
using Rhino.Mocks;
using Wallboard.Tasks;
using Wallboard.Tasks.JsonSerialize;

namespace Wallboard.Tests.Tasks
{
    [TestFixture]
    public class JiraTasksTest
    {
        private IRestClient _restClient;
        private JiraTasks _tasks;

        [SetUp]
        public void Setup()
        {
            _restClient = MockRepository.GenerateMock<IRestClient>();
            _tasks = new JiraTasks();
        }

        [Test]
        public void Constructor_Sets_Defaults()
        {
            //Assert
            var client = _tasks.GetField("_restClient") as IRestClient;
            Assert.AreEqual(ConfigurationManager.AppSettings["jiraRestApi"], client.BaseUrl);
            var authenticator = client.Authenticator as HttpBasicAuthenticator;
            Assert.AreEqual(ConfigurationManager.AppSettings["jiraUsername"], authenticator.GetField("_username"));
            Assert.AreEqual(ConfigurationManager.AppSettings["jiraPassword"], authenticator.GetField("_password"));
        }

        [Test]
        public void AllProjectKeysAndNames_Requests_Project_List()
        {
            //Arrange
            _tasks.SetField<JiraTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project"
                && y.RequestFormat == DataFormat.Json)))
                .Return(new RestResponse { Content = ProjectsJson });

            //Act
            var projects = _tasks.AllProjectKeysAndNames();

            //Assert
            Assert.AreEqual(2, projects.Count);
            Assert.AreEqual("United Sugars - Procurement Portal", projects["CLUSCPP"]);
            Assert.AreEqual("United Sugars - Procurement Portal 2", projects["CLUSCPP2"]);
            _restClient.VerifyAllExpectations();

        }

        [Test]
        public void AllProjectKeysAndNames_Requests_Project_List_No_Projects()
        {
            //Arrange
            _tasks.SetField<JiraTasks>("_restClient", _restClient);
            _restClient.Expect(x => x.Execute(Arg<IRestRequest>.Matches(y => y.Method == Method.GET && y.Resource == "project"
                && y.RequestFormat == DataFormat.Json))).Return(new RestResponse { Content = NoProjectsJson });

            //Act
            var projects = _tasks.AllProjectKeysAndNames();

            //Assert
            Assert.AreEqual(0, projects.Count);
            _restClient.VerifyAllExpectations();

        }

        private string ProjectsJson
        {
            get
            {
                return "[{\"self\": \"http://www.example.com/jira/rest/api/2.0/project/EX\",\"key\": \"CLUSCPP\",\"name\": \"United Sugars - Procurement Portal\"},{\"self\": \"http://www.example.com/jira/rest/api/2.0/project/ABC\",\"key\": \"CLUSCPP2\",\"name\": \"United Sugars - Procurement Portal 2\"}]";
            }
        }

        private string NoProjectsJson
        {
            get { return "[]"; }
        }
    }
}
