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
    public class SendMailContactForUserCommand : IRequest<Result>
    {
        public ContactUsEmailModel RequestModel { get; set; }
        public string Domain { get; set; }
        public ContactType? ContactType { set; get; }
    }

    public class SendMailContactForUserCommandHandler : IRequestHandler<SendMailContactForUserCommand, Result>
    {
        private readonly ILogger<SendCompletedEventHandler> _logger;
        private readonly IEmailService _emailService;

        public SendMailContactForUserCommandHandler(
            ILogger<SendCompletedEventHandler> logger,
            IEmailService emailService
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Result> Handle(SendMailContactForUserCommand request, CancellationToken cancellationToken)
        {
            var rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ContactSendMailForUser);

            var requestModel = request.RequestModel;

            var body = BuildTemplate(rawTemplate, request.RequestModel.CustomerName,request.RequestModel.CustomerMessage,request.Domain, request.ContactType);

            string subject = "Contact – Vietnam Homes Luxury Brokerage";
            if (request.ContactType == ContactType.Book)
            {
                subject = "BOOK SHOWING REQUEST";
            }
            else if (request.ContactType == ContactType.Email)
            {
                subject = "Vietnam Homes Luxury Brokerage";
            }
            else if (request.ContactType == ContactType.Question)
            {
                subject = "Ask A Question - Vietnam Homes Luxury Brokerage";
            }
            else if (request.ContactType == ContactType.Contact)
            {
                subject = "Contact – Vietnam Homes Luxury Brokerage";
            }

            var mailMessage = _emailService.BuildMailMessageForSending(subject, body, new List<string> { requestModel.Email }, null, null);
            await _emailService.SendEmail(mailMessage);

            return Result.Success();
        }

        private static string BuildTemplate(string rawTemplate, string fullName,string message,string domain, ContactType? contactType)
        {
            string defaultMessage = "";

            rawTemplate = rawTemplate.Replace("{{fullName}}", fullName);
            rawTemplate = rawTemplate.Replace("{{host}}", domain);

            if (!string.IsNullOrEmpty(message))
            {
                if (contactType == ContactType.Book)
                {
                    defaultMessage = "Thank you for contacting us here at Vietnam Homes Luxury Brokerage regarding this beautiful property. " +
                        "We will contact you shortly about your Book Showing request.";
                    rawTemplate = rawTemplate.Replace("{{button}}", "");
                }
                else if (contactType == ContactType.Email)
                {
                    defaultMessage = "Thank you for contacting us here at Vietnam Homes Luxury Brokerage regarding this beautiful property. " +
                        "We will contact you shortly about your email.";
                    rawTemplate = rawTemplate.Replace("{{button}}", "");
                }
                else if (contactType == ContactType.Contact || contactType == ContactType.Question)
                {
                    defaultMessage = "Thank you for contacting us here at Vietnam Homes Luxury Brokerage. Your message is important to us and we will be in touch soon. " +
                        "In the meantime, please visit our pre-construction";

                    string button = "<a class='confirm-button' href='https://projects.vietnamhomes.ca/' target='_blank'>Beachside Villas & Condos</a>";
                    rawTemplate = rawTemplate.Replace("{{button}}", button);
                }
            }
            else
            {
                defaultMessage = message;
            }

            rawTemplate = rawTemplate.Replace("{{message}}", defaultMessage);

            return rawTemplate;
        }
    }
}
