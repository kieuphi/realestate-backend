using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Common.Extensions;
using General.Application.Interfaces;
using General.Application.Common.Results;

namespace General.Application.ManageUser.Commands
{
    public class ResetUsersPasswordCommand : IRequest<List<ResetPasswordResult>>
    {
        public List<string> UserEmails { get; set; }
    }

    public class ResetUsersPasswordCommandHandler : IRequestHandler<ResetUsersPasswordCommand, List<ResetPasswordResult>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public ResetUsersPasswordCommandHandler(
            ILogger<CreateUserCommandHandler> logger,
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ResetPasswordResult>> Handle(ResetUsersPasswordCommand request, CancellationToken cancellationToken)
        {
            List<ResetPasswordResult> results = new List<ResetPasswordResult>();

            foreach (var item in request.UserEmails)
            {
                var user = await _identityService.GetUserByIdentifierAsync(item);
                (var resetPasswordResult, string newPassword) = await _identityService.GenerateNewPasswordAsync(user.Email);

                if (!resetPasswordResult.Succeeded)
                {
                    _logger.LogError("Failed to create user with email {0}", item);
                }

                results.Add(new ResetPasswordResult 
                {
                    Email = user.Email,
                    Password = newPassword.ToBase64Encode(),
                    UserName = user.UserName
                });
            }

            return results;
        }
    }
}
