using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;
using CommunicationApiClient = Common.Shared.Microservice.CommunicationClient;
using Common.Shared.Services;

namespace General.Application.ManageUser.Commands
{
    public class UnlockUserCommand : IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly CommunicationApiClient.ICommunicationApiClient _communicationApiClient;

        public UnlockUserCommandHandler(IIdentityService identityService, IClientFileFactoryService clientFileFactoryService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));

            _communicationApiClient = clientFileFactoryService.GetCommunicationApiClient();
        }

        public async Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.UnlockUserAsync(request.UserId);
            if (result.Succeeded == true)
            {
                var user = await _identityService.GetUserByIdentifierAsync(request.UserId);
                var lockAccount = new CommunicationApiClient.LockAccountModel
                {
                    Email = user.Email,
                    FullName = user.Employee
                };
                await _communicationApiClient.ApiCommunicationEmailnotificationUnlockAccountAsync(lockAccount);
            }
            return result;
        }
    }
}
