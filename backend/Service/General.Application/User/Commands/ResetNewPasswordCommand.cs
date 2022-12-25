using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;
using General.Domain.Models;

namespace General.Application.User.Commands
{
    public class ResetNewPasswordCommand : IRequest<Result>
    {
        public ResetPasswordModel ResetPasswordModel { get; set; }
    }

    public class ResetNewPasswordCommandHandler : IRequestHandler<ResetNewPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ResetNewPasswordCommandHandler(
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(ResetNewPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.ResetPasswordModel.Email);
            if (user == null)
            {
                return Result.Failure("userDoesNotExist");
            }
            var result = await _identityService.ResetPasswordAsync(user,request.ResetPasswordModel.Token,request.ResetPasswordModel.NewPassword);

            return result;
        }
    }
}
