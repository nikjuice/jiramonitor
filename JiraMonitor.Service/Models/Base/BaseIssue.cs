namespace JiraMonitor.Service.Models.Base
{
    public class BaseIssue
    {
       public string Key { get; set; }
       public string Summary { get; set; }

       public BaseIssue(string key, string summary)
        {
            Key = key;
            Summary = summary;
        }
    }
}
