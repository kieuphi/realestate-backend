using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;

namespace General.Application.Profile.Commands
{
    public class UpdatePasswordCommand : IRequest<Result>
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string CurrentUserId { get; set; }
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        
        public UpdatePasswordCommandHandler(
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.ChangePasswordAsync(
                request.UserName,
                request.OldPassword,
                request.NewPassword);

            return result;
        }
    }
}
