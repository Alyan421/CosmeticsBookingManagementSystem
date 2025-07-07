using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CMS.Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            var smtpHost = configuration["Smtp:Host"];
            var smtpPort = int.Parse(configuration["Smtp:Port"]);
            var smtpUser = configuration["Smtp:User"];
            var smtpPass = configuration["Smtp:Pass"];
            _fromEmail = configuration["Smtp:FromEmail"];

            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage(_fromEmail, email, subject, message);
                return _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Failed to send email.", ex);
            }
        }
    }
}