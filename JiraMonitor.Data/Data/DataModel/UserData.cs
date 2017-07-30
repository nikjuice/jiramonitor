using JiraMonitor.Data.DbModel;
using System.Collections.Generic;

namespace JiraMonitor.Web.Models
{
    public class UserData
    {
        public ICollection<JqlQuery> JqlQueries { get; set; }
        public EmailNotificationSettings EmailNotificationSettings { get; set; }
        public JiraSettings JiraSettings { get; set; }
        public string UserLogin;
    }
}
