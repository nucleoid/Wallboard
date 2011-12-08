
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using Wallboard.Tasks.JsonSerialize;

namespace Wallboard.Tasks
{
    public class BambooTasks : IBambooTasks
    {
        private const string ProjectResource = "project";
        private const string BuildResource = "result";
        private const string FailedKey = "Failed";

        private readonly IRestClient _restClient;

        public BambooTasks()
        {
            _restClient = new RestClient
            {
                BaseUrl = ConfigurationManager.AppSettings["buildServerRestApi"],
                Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["jiraUsername"], 
                    ConfigurationManager.AppSettings["jiraPassword"])
            };
        }

        public Dictionary<string, string> AllProjectKeysAndNames()
        {
            var request = new RestRequest(MakeJsonRequest(ProjectResource));
            var response = _restClient.Execute(request);
            if(response.Content.Length <= 2)
                return new Dictionary<string, string>();
            var projects = JsonConvert.DeserializeObject<BambooProjectsCollection>(response.Content).Projects.Project.ToDictionary(proj => proj.Key, proj => proj.Name);
            return projects;
        }

        public bool ProjectIsGreen(string projectKey)
        {
            var request = new RestRequest(MakeJsonRequest(string.Format("{0}/{1}", BuildResource, projectKey)));
            var response = _restClient.Execute(request);

            var states = JsonConvert.DeserializeObject<BuildResultsCollection>(response.Content).Results.Result.Select(x => x.State);
            return !states.Any(x => FailedKey == x);
        }

        public IEnumerable<string> FailedPlanDetails(string projectKey)
        {
            var request = new RestRequest(MakeJsonRequest(string.Format("{0}/{1}", BuildResource, projectKey)));
            var response = _restClient.Execute(request);

            var failures = JsonConvert.DeserializeObject<BuildResultsCollection>(response.Content).Results.Result.Where(x => x.State == FailedKey);
            var failedPlanDetails = new List<string>();
            foreach (var result in failures)
                AddPlanDetail(failedPlanDetails, result);
            return failedPlanDetails;
        }

        public IEnumerable<string> ProjectStatuses()
        {
            var projects = AllProjectKeysAndNames();
            var allDetails = new List<string>();
            foreach (var project in projects)
            {
                if(ProjectIsGreen(project.Key))
                    allDetails.Add(string.Format("Project {0} has succeeded", project.Value));
                else
                    allDetails.AddRange(FailedPlanDetails(project.Key));
            }
            return allDetails;
        }

        private void AddPlanDetail(List<string> failedPlanDetails, BuildResult result)
        {
            var detailRequest = new RestRequest(MakeJsonRequest(string.Format("{0}/{1}", BuildResource, result.Key)));
            var detailResponse = _restClient.Execute(detailRequest);
            var detail = JsonConvert.DeserializeObject<BuildProjectResults>(detailResponse.Content);
            failedPlanDetails.Add(string.Format("{0} has FAILED : {1}", result.Key, detail.BuildReason));
        }

        private string MakeJsonRequest(string resource)
        {
            return string.Format("{0}.json", resource);
        }
    }
}