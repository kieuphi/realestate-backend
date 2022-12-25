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
using General.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace General.Application.Email.Commands
{
    public class SendMailContactForSellerCommand : IRequest<Result>
    {
        public BookShowingEmailModel RequestModel { get; set; }
        public string Domain { get; set; }
    }

    public class SendMailContactForSellerCommandHandler : IRequestHandler<SendMailContactForSellerCommand, Result>
    {
        private readonly ILogger<SendCompletedEventHandler> _logger;
        private readonly IEmailService _emailService;


        public SendMailContactForSellerCommandHandler(
            ILogger<SendCompletedEventHandler> logger,
            IEmailService emailService
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Result> Handle(SendMailContactForSellerCommand request, CancellationToken cancellationToken)
        {
            var requestModel = request.RequestModel;
            var rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ContactSendMailForSeller);
            if(string.IsNullOrEmpty(requestModel.ProjectName) == false)
            {
                rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ContactSendMailProjectForSeller);
            }

            var body = BuildTemplate(rawTemplate, requestModel,request.Domain);

            var mailMessage = _emailService.BuildMailMessageForSending(requestModel.Subject + " – Vietnam Homes Luxury Brokerage", body, new List<string> { requestModel.SellerEmail }, requestModel.ListEmailCC, null);
            await _emailService.SendEmail(mailMessage);

            return Result.Success();
        }

        private static string BuildTemplate(string rawTemplate, BookShowingEmailModel requestModel, string domain)
        {
            rawTemplate = rawTemplate.Replace("{{sellerName}}", requestModel.SellerName);
            rawTemplate = rawTemplate.Replace("{{contactType}}", requestModel.ContactType);
            rawTemplate = rawTemplate.Replace("{{customerName}}", requestModel.CustomerName);
            rawTemplate = rawTemplate.Replace("{{email}}", requestModel.CustomerEmail);

            rawTemplate = rawTemplate.Replace("{{phone}}", requestModel.CustomerPhone);
            rawTemplate = rawTemplate.Replace("{{message}}", requestModel.CustomerMessage);
            rawTemplate = rawTemplate.Replace("{{propertyNumber}}", requestModel.PropertyNumber);
            rawTemplate = rawTemplate.Replace("{{propertyTitle}}", requestModel.PropertyTitle);

            rawTemplate = rawTemplate.Replace("{{transactionType}}", requestModel.TransactionType);
            rawTemplate = rawTemplate.Replace("{{propertyType}}", requestModel.PropertyType);

            rawTemplate = rawTemplate.Replace("{{projectName}}", requestModel.ProjectName);
            rawTemplate = rawTemplate.Replace("{{province}}", requestModel.Province);
            rawTemplate = rawTemplate.Replace("{{district}}", requestModel.District);

            rawTemplate = rawTemplate.Replace("{{host}}", domain);

            return rawTemplate;
        }


    }
}
