using General.Application.Interfaces;
using General.Domain.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Common.Shared.Models;
using System.IO;
using System.Net;
using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace General.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly AppSettings _appSettings;
        private readonly IEnvironmentApplication _environmentApplication;
        private readonly IApplicationDbContext _context;

        public EmailService(
            ILogger<EmailService> logger,
            IOptions<AppSettings> options,
            IEnvironmentApplication environmentApplication,
            IApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environmentApplication = environmentApplication ?? throw new ArgumentNullException(nameof(environmentApplication));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = options.Value;
        }

        public async Task SendEmail(MailMessage mailMessage)
        {
            try
            {
                var config = GetConfig();
                var client = CreateSMTPClient(
                    config.UserName,
                    config.Password,
                    config.Port,
                    config.Host);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }
        }
        public async Task<Result> SendEmailWithReturnResult(MailMessage mailMessage)
        {
            Result result = Result.Failure("Failed to send email");

            try
            {
                var config = GetConfig();
                var client = CreateSMTPClient(
                    config.UserName,
                    config.Password,
                    config.Port,
                    config.Host);

                await client.SendMailAsync(mailMessage);

                result = Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
            }

            return result;
        }
        public async Task<string> GetEmailTemplate(string templateName)
        {
            string templateEmail = string.Empty;
            using (StreamReader reader = new StreamReader(_environmentApplication.WebRootPath + ($"/EmailTemplates/{templateName}.html")))
            {
                templateEmail = await reader.ReadToEndAsync();
            }

            return templateEmail;
        }

        public MailMessage BuildMailMessageForSending(string subject, string body, List<string> emailTos, List<string> ccEmails, List<string> bccEmails)
        {
            var config = GetConfig();
            var message = new MailMessage
            {
                From = new MailAddress(config.UserName, config.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            if (emailTos != null)
            {
                foreach (var item in emailTos)
                {
                    message.To.Add(new MailAddress(item, item));
                }
            }

            if (ccEmails != null)
            {
                foreach (var item in ccEmails)
                {
                    message.CC.Add(new MailAddress(item, item));
                }
            }

            if (bccEmails != null)
            {
                foreach (var item in bccEmails)
                {
                    message.Bcc.Add(new MailAddress(item, item));
                }

            }

            return message;
        }

        private static SmtpClient CreateSMTPClient(string username, string password,
            int port = 587,
            string host = "smtp.gmail.com",
            bool enableSsl = true)
        {
            return new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password)
            };
        }

        private ConfigEntity GetConfig()
        {
            return (_context.Config.FirstOrDefault());
        }
    }
}
