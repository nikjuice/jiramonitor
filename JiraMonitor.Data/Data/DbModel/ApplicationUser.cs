using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace JiraMonitor.Data.DbModel

{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<JqlQuery> JqlQueries { get; set; }
        public JiraSettings JiraSettings { get; set; }
        public EmailNotificationSettings EmailNotificaionSettings { get; set; }
    }

    public class JqlQuery 
    {
        public int ID { get; set; }
        public string ApplicationUserId { get; set; }       

        public string Query { get; set; }
        public int PollingInterval { get; set; }

        public ICollection<TrackedJiraIssue> trackedIssues { get; set; }
    }

    public class JiraSettings
    {
        public int ID { get; set; }
        public string ApplicationUserId { get; set; }        

        public string Url { get; set; }
        public string User { get; set; }

        //TODO use secure string for password
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class EmailNotificationSettings
    {
        public int ID { get; set; }
        public string ApplicationUserId { get; set; }        

        public string Email { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }

        public string EmailTo { get; set; }

        //TODO use secure string for password
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

