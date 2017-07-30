
namespace JiraMonitor.Service.Services
{
   public  interface IEmailSender
    {
        void Authenticate(string smtpServer, int smptPort, string user, string password);
        void SendMessage(string email, string subject, string body, string from);
    }
}
