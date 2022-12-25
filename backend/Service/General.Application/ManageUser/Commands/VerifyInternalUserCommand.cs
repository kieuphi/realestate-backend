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
using Newtonsoft.Json;

namespace General.Application.ManageUser.Commands
{
    public class VerifyInternalUserCommand : IRequest<Result>
    {
        public VerifyAccountModel VerifyAccountModel { get; set; }
    }

    public class VerifyInternalUserCommandHandler : IRequestHandler<VerifyInternalUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<VerifyInternalUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly AccessManagement.IAccessManagementApiClient _accessManagementApiClient;
        public VerifyInternalUserCommandHandler(
            ILogger<VerifyInternalUserCommandHandler> logger,
             IIdentityService identityService,
             IMapper mapper, IClientFileFactoryService clientFileFactoryService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _accessManagementApiClient = clientFileFactoryService.GetAccessManagementApiClient();
        }

        public async Task<Result> Handle(VerifyInternalUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Verify account request "+JsonConvert.SerializeObject(request));
            var user = await _identityService.GetUserByEmailAsync(request.VerifyAccountModel.Email);
            if (user == null)
            {
                return Result.Failure("UserNotExist");
            }

            Result result = await _identityService.ConfirmEmailAsync(user, request.VerifyAccountModel.Token);
            _logger.LogInformation("Verify account result " + JsonConvert.SerializeObject(result));
            if (result.Succeeded == false)
            {
                return result;
            }
            //if (request.VerifyAccountModel.NewPassword != request.VerifyAccountModel.ConfirmPassword)
            //{
            //    return Result.Failure("PasswordAndConfirmPasswordNotMatch");
            //}

            //result = await _identityService.AddPasswordAsync(user, request.VerifyAccountModel.ConfirmPassword);

            return result;
        }
        
    }
}
