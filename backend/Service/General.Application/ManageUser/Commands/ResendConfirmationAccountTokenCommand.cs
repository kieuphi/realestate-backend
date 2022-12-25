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
    public class ResendConfirmationAccountTokenCommand : IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class ResendConfirmationAccountTokenCommandHandler : IRequestHandler<ResendConfirmationAccountTokenCommand, Result>
    {
        private readonly CommunicationApiClient.ICommunicationApiClient _communicationApiClient;
        private readonly IIdentityService _identityService;
        private readonly ILogger<ResendConfirmationAccountTokenCommandHandler> _logger;
        private readonly IMapper _mapper;
        public ResendConfirmationAccountTokenCommandHandler(
                ILogger<ResendConfirmationAccountTokenCommandHandler> logger,
                IIdentityService identityService,
                IClientFileFactoryService clientFileFactoryService,
                IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _communicationApiClient = clientFileFactoryService.GetCommunicationApiClient();
        }

        public async Task<Result> Handle(ResendConfirmationAccountTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdentifierAsync(request.UserId);
            if (user == null)
            {
                return Result.Failure("UserNotExist");
            }
            if (user.EmailConfirmed == true)
            {
                return Result.Failure("AccountWasVerified");
            }

            var sendMailResult = await ConfirmationAccountUsingVerifyLink(user);
            if (sendMailResult.Succeeded == false)
            {
                return sendMailResult;
            }

            return Result.Success();
        }
        private async Task<Result> ConfirmationAccountUsingVerifyLink(ApplicationUser user)
        {
            string token = await _identityService.GenerateEmailConfirmationTokenAsync(user);
            var request = new CommunicationApiClient.ConfirmationAccountModel()
            {
                UserName = user.UserName,
                FullName = user.Employee,
                Identifier = user.Email,
                Token = token
            };
            try
            {
                var result = await _communicationApiClient.ApiCommunicationEmailnotificationVerifyAccountAsync(request);
                if (result.Succeeded == false)
                {
                    return Result.Failure(result.Errors.ToList());
                }
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }
        
    }
}
