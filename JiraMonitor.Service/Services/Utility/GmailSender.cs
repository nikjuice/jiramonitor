using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace JiraMonitor.Service.Services
{
    public class GmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private SmtpClient _client;

        public GmailSender(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BasicJqlExecutor>();
            _client = new SmtpClient();           
        }
    

        public void Authenticate(string smtpServer, int smptPort, string user, string password)
        {
            try
            {
                _client.Connect(smtpServer, smptPort);
                _client.AuthenticationMechanisms.Remove("XOAUTH2");

                _client.Authenticate(user, password);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error during connection and authentication to {0}, exception - {1} ", smtpServer, ex.StackTrace);
            }
        }

        public void SendMessage(string email, string subject, string body, string from)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(from, from));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("plain") { Text = body };

                _client.Send(emailMessage);

                _client.Disconnect(true);

                _logger.LogInformation("Message sent to {0}", email);
            }

            catch (Exception ex)
            {
                _logger.LogError("Error during sending mail to {0}, Exception - {1} ", email, ex.StackTrace);
            }
        }
    }
}
