using JiraMonitor.Service.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JiraMonitor.Service.Services
{
    public interface IJqlExecutor
    {
        void Authenticate(string baseUrl, string user, string password);
        Task<IEnumerable<BaseIssue>> ExecuteJqlAsync(string query);
    }
}
