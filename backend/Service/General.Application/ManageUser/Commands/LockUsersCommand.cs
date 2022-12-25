using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;

namespace General.Application.ManageUser.Commands
{
    public class LockUsersCommand : IRequest<Result>
    {
        public List<string> Users { get; set; }
    }

    public class LockUsersCommandHandler : IRequestHandler<LockUsersCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public LockUsersCommandHandler(
             IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(LockUsersCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Users)
            {
                await _identityService.LockUserAsync(item);
            }
            
            return Result.Success();
        }
    }
}
