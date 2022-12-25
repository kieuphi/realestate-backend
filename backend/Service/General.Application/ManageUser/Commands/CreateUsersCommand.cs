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
using General.Domain.Models;
using Common.Shared.Services;

namespace General.Application.ManageUser.Commands
{
    public class CreateUsersCommand : IRequest<List<CreateUserResult>>
    {
        public List<CreateUserModel> Users { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUsersCommand, List<CreateUserResult>>
    {
        private readonly CommunicationApiClient.ICommunicationApiClient _communicationApiClient;
        private readonly IIdentityService _identityService;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
             ILogger<CreateUserCommandHandler> logger,
             IIdentityService identityService,
             IClientFileFactoryService clientFileFactoryService,
             IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _communicationApiClient = clientFileFactoryService.GetCommunicationApiClient();
        }

        public async Task<List<CreateUserResult>> Handle(CreateUsersCommand request, CancellationToken cancellationToken)
        {
            List<CreateUserResult> createUserResults = new List<CreateUserResult>();
            var emailNotifications = new List<CommunicationApiClient.CreateUserModel>();

            foreach (var item in request.Users)
            {
                (var result, string temporaryPassword) = await _identityService.CreateUserWithTemporaryPasswordAsync(item.UserName, item.Email, item.PhoneNumber, item.RoleDefinitionId, item);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create user with email {0}", item.Email);
                }
                
                var userIdentity = new CreateUserResult()
                {
                    Email = item.Email,
                    UserName = item.Email,
                    Password = temporaryPassword
                };
                createUserResults.Add(userIdentity);
                emailNotifications.Add(_mapper.Map<CommunicationApiClient.CreateUserModel>(userIdentity));

            }

            //PRODUCTION EXPECTED
            await _communicationApiClient.ApiCommunicationEmailnotificationUsersAsync(emailNotifications);

            return createUserResults;
        }
    }
}
