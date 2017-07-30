using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace JiraMonitor.Service.Models.Jira
{
        public class SearchResponse
        {
            [JsonProperty("expand")]
            public string Expand { get; set; }

            [JsonProperty("startAt")]
            public int StartAt { get; set; }

            [JsonProperty("maxResults")]
            public int MaxResults { get; set; }

            [JsonProperty("total")]
            public int Total { get; set; }

            [JsonProperty("issues")]
            public List<Issue> Issues { get; set; }
        }


        public class Issue
        {
            [JsonProperty("expand")]
            public string Expand { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }
       
            [JsonProperty("key")]
            public String Key { get; set; }
      
            [JsonProperty("fields")]
            public Fields Fields { get; set; }
        }        

        public class Fields
        {
            [JsonProperty("summary")]
            public string Summary { get; set; }
       
        }         
    
}
