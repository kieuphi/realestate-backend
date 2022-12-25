using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using General.Application.Interfaces;
using General.Domain.Constants;
using System.Web;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace General.Application.Email.Commands
{
    public class SendForgotPasswordEmailCommand : IRequest<Result>
    {
        public ForgotPasswordModel Model { set; get; }
    }

    public class SendForgotPasswordEmailCommandHandler : IRequestHandler<SendForgotPasswordEmailCommand, Result>
    {
        private readonly ILogger<SendForgotPasswordEmailCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _dbContext;

        public SendForgotPasswordEmailCommandHandler(
            ILogger<SendForgotPasswordEmailCommandHandler> logger,
            IEmailService emailService,
            IResetPasswordService resetPasswordService,
            ICommonFunctionService commonFunctionService,
            IIdentityService identityService,
            IApplicationDbContext dbContext
        ) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _resetPasswordService = resetPasswordService ?? throw new ArgumentNullException(nameof(resetPasswordService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Result> Handle(SendForgotPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            var rawTemplate = await _emailService.GetEmailTemplate(EmailTemplate.ForgotPassword);
            var requestModel = request.Model;
            string host = _commonFunctionService.ConvertImageUrl("");

            var users = await _identityService.GetUsersAsync();
            var userProfile = (await _dbContext.ProfileInformation
                .ToListAsync())
                .Where(x => users.Any(y => y.Id == x.UserId.ToString() && y.Email == request.Model.Email))
                .FirstOrDefault();
            var firstName = userProfile != null ? userProfile.FirstName : request.Model.Email;

            if (string.IsNullOrWhiteSpace(requestModel.Email) || string.IsNullOrWhiteSpace(requestModel.Token))
            {
                _logger.LogError($"Failed when send email to user");
                return Result.Failure($"Invalid request model");
            }

            var resetPath = $"?email=" + HttpUtility.UrlEncode(requestModel.Email) + $"&token=" + HttpUtility.UrlEncode(requestModel.Token);

            var url = _resetPasswordService.ResetPasswordUrl + resetPath;
            if (requestModel.UserRole == "InternalUser")
            {
                url = _resetPasswordService.ResetPasswordInternalUserUrl + resetPath;
            }
            var body = BuildTemplate(rawTemplate, requestModel.Email, userProfile.FirstName, url, host);

            var mailMessage = _emailService.BuildMailMessageForSending("Reset Password - Vietnam Homes Luxury Brokerage", body, new List<string> { requestModel.Email }, null, null);
            _ = _emailService.SendEmail(mailMessage);

            return Result.Success();
        }

        private static string BuildTemplate(string rawTemplate, string email, string firstName, string resetPassworkLink, string host)
        {
            rawTemplate = rawTemplate.Replace("{{email}}", email);
            rawTemplate = rawTemplate.Replace("{{firstName}}", firstName);
            rawTemplate = rawTemplate.Replace("{{resetPassworkLink}}", resetPassworkLink);
            rawTemplate = rawTemplate.Replace("{{host}}", host);

            return rawTemplate;
        }
    }
}
