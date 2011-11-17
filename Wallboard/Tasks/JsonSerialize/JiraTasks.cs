using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace Wallboard.Tasks.JsonSerialize
{
    public class JiraTasks : IJiraTasks
    {
        private const string ProjectResource = "project";

        private readonly IRestClient _restClient;

        public JiraTasks()
        {
            _restClient = new RestClient
            {
                BaseUrl = ConfigurationManager.AppSettings["jiraRestApi"], 
                Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["jiraUsername"], ConfigurationManager.AppSettings["jiraPassword"])
            };
        }

        public Dictionary<string, string> AllProjectKeysAndNames()
        {
            var request = new RestRequest(ProjectResource)
            {
                RequestFormat = DataFormat.Json
            };
            var response = _restClient.Execute(request);

            var projects = JsonConvert.DeserializeObject<IEnumerable<JiraProject>>(response.Content).ToDictionary(proj => proj.Key, proj => proj.Name);
            return projects;
        }
    }
}