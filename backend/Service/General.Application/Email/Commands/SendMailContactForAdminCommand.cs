using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Common.Shared.Models;
using General.Application.Interfaces;
using General.Domain.Constants;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace General.Application.Email.Commands
{
    public class SendMailContactForAdminCommand : IRequest<Result>
    {
        public ContactUsEmailModel RequestModel { get; set; }
        public List<string> ListEmail { get; set; }
        public string Domain { get; set; }
        public ContactType ContactType { set; get; }
    }

    public class SendMailContactForAdminCommandHandler : IRequestHandler<SendMailContactForAdminCommand, Result>
    {
        private readonly ILogger<SendCompletedEventHandler> _logger;
        private readonly IEmailService _emailService;

        public SendMailContactForAdminCommandHandler(
            ILogger<SendCompletedEventHandler> logger,
            IEmailService emailService
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Result> Handle(SendMailContactForAdminCommand request, CancellationToken cancellationToken)
        {
            var rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ContactSendMailForAdmin);

            var requestModel = request.RequestModel;

            var body = BuildTemplate(rawTemplate, requestModel,request.Domain);

            string subject = "Contact – Vietnam Homes Luxury Brokerage - ";
            if (request.ContactType == ContactType.Question)
            {
                subject = "Ask A Question - Vietnam Homes Luxury Brokerage - ";
            }
            else if (request.ContactType == ContactType.Contact)
            {
                subject = "Contact – Vietnam Homes Luxury Brokerage - ";
            }

            var mailMessage = _emailService.BuildMailMessageForSending(subject + requestModel.Subject, body, request.ListEmail, null, null);
            await _emailService.SendEmail(mailMessage);

            return Result.Success();
        }

        private static string BuildTemplate(string rawTemplate, ContactUsEmailModel requestModel, string domain)
        {
            rawTemplate = rawTemplate.Replace("{{customerName}}", requestModel.CustomerName);
            rawTemplate = rawTemplate.Replace("{{email}}", requestModel.CustomerEmail);
            rawTemplate = rawTemplate.Replace("{{phone}}", requestModel.CustomerPhone);
            rawTemplate = rawTemplate.Replace("{{message}}", requestModel.CustomerMessage);
            rawTemplate = rawTemplate.Replace("{{host}}", domain);

            return rawTemplate;
        }
    }
}
