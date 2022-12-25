using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Common.Extensions;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Enums = Common.Shared.Enums;
using AutoMapper;
using CommunicationApiClient = Common.Shared.Microservice.CommunicationClient;
using AccessManagement = Common.Shared.Microservice.AccessManagementClient;
using General.Domain.Models;
using Common.Shared.Models;
using General.Domain.Common;
using General.Domain.Enums;
using System.Linq;
using Common.Shared.Services;

namespace General.Application.ManageUser.Commands
{
    public class UpdateInternalUserCommand : IRequest<Result>
    {
        public UserModel User { get; set; }
    }

    public class UpdateInternalUserCommandHandler : IRequestHandler<UpdateInternalUserCommand, Result>
    {
        private readonly CommunicationApiClient.ICommunicationApiClient _communicationApiClient;
        private readonly IIdentityService _identityService;
        private readonly ILogger<UpdateInternalUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        public UpdateInternalUserCommandHandler(
                ILogger<UpdateInternalUserCommandHandler> logger,
                IIdentityService identityService,
                IClientFileFactoryService clientFileFactoryService,
                IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _communicationApiClient = clientFileFactoryService.GetCommunicationApiClient();
        }

        public async Task<Result> Handle(UpdateInternalUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdentifierAsync(request.User.Id);

            if (user == null)
            {
                return Result.Failure("UserNotExist");
            }

            user.Employee = request.User.Employee;
            user.PhoneNumber = request.User.PhoneNumber;
            user.Email = request.User.Email;

            var result = await _identityService.UpdateUserAsync(user);

            return result;
        }
       
    }
}
