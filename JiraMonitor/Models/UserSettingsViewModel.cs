using JiraMonitor.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraMonitor.Model
{
    public class UserSettingsViewModel
    {
        public EmailNotificationSettings emailNotificationSettings { get; set; }
        public JiraSettings jiraSettings { get; set; }
    }
}
