using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JiraMonitor.Data.DbModel
{
    public class TrackedJiraIssue
    {
        public int ID { get; set; }
        public int JqlQueryId { get; set; }
        [MaxLength(20)]
        public string JiraIssueKey { get; set; }

    }
}
