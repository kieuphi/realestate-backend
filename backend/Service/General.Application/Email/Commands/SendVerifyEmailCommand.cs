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
    public class SendVerifyEmailCommand : IRequest<Result>
    {
        public ConfirmationAccountModel RequestModel { get; set; }
    }

    public class SendVerifyEmailCommandHandler : IRequestHandler<SendVerifyEmailCommand, Result>
    {
        private readonly ILogger<SendCompletedEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly ISettingValueService _settingValueService;
        private readonly ICommonFunctionService _commonFunctionService;

        public SendVerifyEmailCommandHandler(
            ILogger<SendCompletedEventHandler> logger,
            IEmailService emailService,
            ISettingValueService settingValueService,
            ICommonFunctionService commonFunctionService
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _settingValueService = settingValueService ?? throw new ArgumentNullException(nameof(settingValueService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ConfirmationAccount);
            string host = _commonFunctionService.ConvertImageUrl("");

            var requestModel = request.RequestModel;

            if (string.IsNullOrWhiteSpace(requestModel.Token))
            {
                _logger.LogError($"Failed when send email to user");
                return Result.Failure($"Invalid request model");
            }

            var resetPath = $"?email=" + HttpUtility.UrlEncode(requestModel.Email) + $"&token=" + HttpUtility.UrlEncode(requestModel.Token);
            var url = _settingValueService.VerifyAccountUrl + resetPath;
            var body = BuildTemplate(rawTemplate, requestModel.Email, requestModel.FirstName, requestModel.LastName, url, host);

            var mailMessage = _emailService.BuildMailMessageForSending("Activate Your Account – Vietnam Homes Luxury Brokerage", body, new List<string> { requestModel.Email }, null, null);
            await _emailService.SendEmail(mailMessage);

            return Result.Success();
        }

        private static string BuildTemplate(string rawTemplate, string email, string firstName, string lastName, string link, string host)
        {
            rawTemplate = rawTemplate.Replace("{{FirstName}}", firstName);
            rawTemplate = rawTemplate.Replace("{{LastName}}", lastName);
            rawTemplate = rawTemplate.Replace("{{Email}}", email);
            rawTemplate = rawTemplate.Replace("{{VerifyLink}}", link);
            rawTemplate = rawTemplate.Replace("{{host}}", host);

            return rawTemplate;
        }
    }
}
