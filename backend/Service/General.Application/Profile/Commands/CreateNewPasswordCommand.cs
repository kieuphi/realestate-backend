using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;

namespace General.Application.Profile.Commands
{
    public class CreateNewPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class CreateNewPasswordCommandHandler : IRequestHandler<CreateNewPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public CreateNewPasswordCommandHandler(
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(CreateNewPasswordCommand request, CancellationToken cancellationToken)
        {
            (var result, _) = await _identityService.GenerateNewPasswordAsync(request.Email);
            return result;
        }
    }
}
