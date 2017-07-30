using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using JiraMonitor.Web.Models;
using JiraMonitor.Data;
using JiraMonitor.Service.Models.Base;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JiraMonitor.Data.DbModel;

namespace JiraMonitor.Service.Services
{
    public class JiraPoller : IJiraPoller
    {
        private IEnumerable<UserData> _usersdata;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;

        private readonly IServiceProvider _serviceProvider;
  
        public void Initialize(IEnumerable<UserData> usersdata)
        {
            if (usersdata != null && usersdata.Count() > 0)
            {
                _usersdata = usersdata;
            }
            else
            {
                _logger.LogInformation("No data to work");
            }
        }

        public JiraPoller(ApplicationDbContext dbContext, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;     

            _logger = loggerFactory.CreateLogger<JiraPoller>();
            _serviceProvider = serviceProvider;           
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (_usersdata != null && _usersdata.Count() > 0)
            {
                _logger.LogInformation("Service started.");

                List<Task> tasks = new List<Task>();

                foreach (var userData in _usersdata)
                {
                    if (isUserDataValid(userData))
                    {
                        foreach (var jqlQuery in userData.JqlQueries)
                        {
                            try
                            {
                                tasks.Add(Task.Run(() => ExecuteAndCheckJql(jqlQuery, userData.EmailNotificationSettings, userData.JiraSettings)));
                            }

                            catch (Exception ex)
                            {
                                _logger.LogError("Exception occured during adding task for user - {0}, query - {1}, Exception - {2}", "bla", jqlQuery.Query, ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("User data is not valid, email - {0}", userData.UserLogin);
                    }
                }

                Task.WaitAll();
            }
                     
        }

        private bool isUserDataValid(UserData userData)
        {
            if (userData.JqlQueries != null && userData.JqlQueries.Count > 0)
            {
                if (userData.EmailNotificationSettings != null && userData.JiraSettings != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExecuteAndCheckJql(JqlQuery query, EmailNotificationSettings emailSettings, JiraSettings jiraSettings)
        {
            _logger.LogInformation("Creating execution helper for query with id = {0} and set a timer {1} ms for it, query - {2}", query.ID, query.PollingInterval, query.Query);

            try
            {
                var helper = new ExecutionHelper(query, emailSettings, jiraSettings, _serviceProvider, _logger);

                var _autoEvent = new AutoResetEvent(false);

                Timer taskTimer = new Timer(helper.Execute, _autoEvent, query.PollingInterval, query.PollingInterval);

                _autoEvent.WaitOne();
            }

            catch (Exception ex)
            {
                _logger.LogError("Exception occured during trying to schedule a task, exception, Exception - {0}", ex.StackTrace);
            }
        }

        private class ExecutionHelper
        {
            private readonly EmailNotificationSettings _emailNotificationSettings;
            private readonly JiraSettings _jiraNotificationSettings;
            private JqlQuery _query;

            private readonly IEmailSender _emailSender;
            private readonly IJqlExecutor _jqlExecutor;
            private readonly IServiceProvider _serviceProvider;
            private readonly ILogger _logger;

            private readonly ApplicationDbContext _dbContext;


            public ExecutionHelper(JqlQuery query, EmailNotificationSettings emailNotificationSettings, JiraSettings jiraNotificationSettings, IServiceProvider serviceProvider, ILogger logger)
            {
                _serviceProvider = serviceProvider;
                _emailNotificationSettings = emailNotificationSettings;
                _jiraNotificationSettings = jiraNotificationSettings;
                _logger = logger;
                _query = query;

                _emailSender = serviceProvider.GetService<IEmailSender>();
                _jqlExecutor = serviceProvider.GetService<IJqlExecutor>();

                _dbContext = serviceProvider.GetService<ApplicationDbContext>();

                _jqlExecutor.Authenticate(_jiraNotificationSettings.Url, _jiraNotificationSettings.User, _jiraNotificationSettings.Password);

            }

            public void Execute(Object state)
            {
                try
                {                    
                    _logger.LogInformation("Execution helper -> trying execute query - id - {0} query - {1}",_query.ID, _query.Query);

                    //for now do not use await here
                    var issues = _jqlExecutor.ExecuteJqlAsync(_query.Query);

                    var newIssues = FindNewIssues(issues.Result);

                    if (newIssues != null && newIssues.Count() > 0)
                    {
                        _logger.LogInformation("Updates found {0} - new issues", newIssues.Count());

                        sendNewIssuesAsEmail(newIssues);
                    }
                    else
                    {
                        _logger.LogInformation("No updates found");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError("Exception during jql execution attempt - {0}", ex.StackTrace);
                }
               
            }

            private IEnumerable<BaseIssue> FindNewIssues(IEnumerable<BaseIssue> issues)
            {
                _dbContext.Entry(_query).Collection(q => q.trackedIssues).Load();

                if (_query.trackedIssues != null)
                {

                    var newIssues = issues.Where(issue => _query.trackedIssues.FirstOrDefault(ti => ti.JiraIssueKey == issue.Key) == null).ToList();

                    newIssues.ForEach(issue =>
                                       _query.trackedIssues.Add(new TrackedJiraIssue() { JqlQueryId = _query.ID, JiraIssueKey = issue.Key })
                                     );

                    _dbContext.SaveChanges();

                    return newIssues;
                }
                else
                {
                    _logger.LogError("Tracked issues entity is not initialized properly");

                    return null;
                }
            }


            private void sendNewIssuesAsEmail(IEnumerable<BaseIssue> newIssues)
            {
                _logger.LogInformation("Execution helper -> trying execute query - id - {0} query - {1}", _query.ID, _query.Query);

                StringBuilder messageBody = new StringBuilder();

                newIssues.ToList().ForEach(ni => messageBody.Append(String.Format("{0} - {1}\r\n", ni.Key, ni.Summary)));

                _emailSender.Authenticate(_emailNotificationSettings.SmtpServer, Convert.ToInt32(_emailNotificationSettings.SmtpPort), _emailNotificationSettings.Email, _emailNotificationSettings.Password);

                _emailSender.SendMessage(_emailNotificationSettings.EmailTo, "Updates found", messageBody.ToString(), _emailNotificationSettings.Email);
            }
        }

    }

    
}
