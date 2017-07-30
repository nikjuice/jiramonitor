using System;
using System.Collections.Generic;
using System.Text;
using JiraMonitor.Service.Models.Base;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using JiraMonitor.Service.Models.Jira;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace JiraMonitor.Service.Services
{
    public class BasicJqlExecutor : IJqlExecutor
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }

        private readonly HttpClient _client;
        private readonly ILogger _logger;

        private string baseApiUrl = "rest/api/2/search?jql=";

        public BasicJqlExecutor(ILoggerFactory loggerFactory)
        {
            _client = new HttpClient();
            _logger = loggerFactory.CreateLogger<BasicJqlExecutor>();
        }

        public void Authenticate(string baseUrl, string user, string password)
        {
            User = user;
            Password = password;
            BaseUrl = baseUrl;

            _client.BaseAddress = new Uri(baseUrl);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
               "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{User}:{Password}")));
        }

        public async Task<IEnumerable<BaseIssue>> ExecuteJqlAsync(string query)
        {
            List<BaseIssue> jiraIssues = new List<BaseIssue>();


            string apiQuery = baseApiUrl + WebUtility.UrlEncode(query) + WebUtility.UrlDecode("&maxResults=500");

            try
            {
                _logger.LogInformation("Executing call to Jira instance, full query - {0}", apiQuery);
                HttpResponseMessage response = await _client.GetAsync(apiQuery);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Response OK");

                    string data = await response.Content.ReadAsStringAsync();

                    SearchResponse jiraResponse = JsonConvert.DeserializeObject<SearchResponse>(data);

                    if (jiraResponse.Issues.Count > 0)
                    {

                        _logger.LogInformation("Fetched {0} jira issues", jiraResponse.Issues.Count);

                        foreach(Issue issue in jiraResponse.Issues)
                        {
                            jiraIssues.Add(new BaseIssue(issue.Key, issue.Fields.Summary));
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No jira issues fetched.");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Expection occurs during jql execution ex - {0}", ex.StackTrace);
            }

            return jiraIssues;
        }      

       
    }
}
