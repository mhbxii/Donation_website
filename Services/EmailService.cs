using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using dotnet9.Interfaces;
using Microsoft.Extensions.Configuration;

namespace dotnet9.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            // Read SMTP settings from configuration
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]!);
            var username = _configuration["EmailSettings:Username"];
            // We're using the SmtpKey as the SMTP password here.
            var password = _configuration["EmailSettings:SmtpKey"];

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail!),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
            };

            message.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = new NetworkCredential(username, password);

                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error sending email: " + ex.Message, ex);
                }
            }
        }
    }
}
