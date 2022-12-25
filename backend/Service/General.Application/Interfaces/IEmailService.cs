using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Common.Shared.Models;

namespace General.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(MailMessage mailMessage);
        Task<Result> SendEmailWithReturnResult(MailMessage mailMessage);
        Task<string> GetEmailTemplate(string templateName);
        MailMessage BuildMailMessageForSending(string subject, string body, List<string> emailTos, List<string> ccEmails, List<string> bccEmails);
    }
}
