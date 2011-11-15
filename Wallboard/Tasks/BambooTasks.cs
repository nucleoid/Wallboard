
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using RestSharp;

namespace Wallboard.Tasks
{
    public class BambooTasks : IBambooTasks
    {
        private const string ProjectResource = "project";
        private const string BuildResource = "result";
        private const string KeyAttribute = "key";
        private const string NameAttribute = "name";
        private const string StateAttribute = "state";
        private const string ReasonAttribute = "buildReason";
        private const string FailedKey = "Failed";

        private readonly IRestClient _restClient;

        public BambooTasks(IRestClient restClient)
        {
            _restClient = restClient;
            _restClient.BaseUrl = ConfigurationManager.AppSettings["buildServerRestApi"];
        }

        public Dictionary<string, string> AllProjectKeysAndNames()
        {
            var request = new RestRequest(MakeXmlRequest(ProjectResource));
            var response = _restClient.Execute(request);

            var document = new XmlDocument();
            document.LoadXml(response.Content);
            var projects = document.GetElementsByTagName(ProjectResource).Cast<XmlNode>()
                .ToDictionary(node => node.Attributes[KeyAttribute].Value, node => node.Attributes[NameAttribute].Value);
            return projects;
        }

        public bool ProjectIsGreen(string projectKey)
        {
            var request = new RestRequest(MakeXmlRequest(string.Format("{0}/{1}", BuildResource, projectKey)));
            var response = _restClient.Execute(request);

            var document = new XmlDocument();
            document.LoadXml(response.Content);
            var states = document.GetElementsByTagName(BuildResource).Cast<XmlNode>().Select(x => x.Attributes[StateAttribute].Value);
            return !states.Any(x => FailedKey == x);
        }

        public IEnumerable<string> FailedPlanDetails(string projectKey)
        {
            var request = new RestRequest(MakeXmlRequest(string.Format("{0}/{1}", BuildResource, projectKey)));
            var response = _restClient.Execute(request);

            var document = new XmlDocument();
            document.LoadXml(response.Content);
            var failures = document.GetElementsByTagName(BuildResource).Cast<XmlNode>().Where(x => x.Attributes[StateAttribute].Value == FailedKey);
            var failedPlanDetails = new List<string>();
            foreach (var node in failures)
                AddPlanDetail(failedPlanDetails, node);
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
                    foreach (var failedPlan in FailedPlanDetails(project.Key))
                        allDetails.Add(failedPlan);
            }
            return allDetails;
        }

        private void AddPlanDetail(List<string> failedPlanDetails, XmlNode node)
        {
            var builder = new StringBuilder(node.Attributes[KeyAttribute].Value).Append(" has FAILED : ");
            var detailRequest = new RestRequest(MakeXmlRequest(string.Format("{0}/{1}", BuildResource, node.Attributes[KeyAttribute].Value)));
            var detailResponse = _restClient.Execute(detailRequest);
            var detailDocument = new XmlDocument();
            detailDocument.LoadXml(detailResponse.Content);
            builder.Append(detailDocument.GetElementsByTagName(ReasonAttribute).Item(0).InnerText);
            failedPlanDetails.Add(builder.ToString());
        }

        private string MakeXmlRequest(string resource)
        {
            return string.Format("{0}.xml", resource);
        }
    }
}