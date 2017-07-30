using JiraMonitor.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiraMonitor.Service.Services
{
    public interface IJiraPoller
    {
        void Initialize(IEnumerable<UserData> usersdata);
        void Start();
        void Pause();        
    }
}
