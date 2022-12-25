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
    public class CreateInternalUserCommand : IRequest<Result>
    {
        public CreateInternalUserModel User { get; set; }
    }

    public class CreateInternalUserCommandHandler : IRequestHandler<CreateInternalUserCommand, Result>
    {
        private readonly CommunicationApiClient.ICommunicationApiClient _communicationApiClient;
        private readonly IIdentityService _identityService;
        private readonly ILogger<CreateInternalUserCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly AccessManagement.IAccessManagementApiClient _accessManagementApiClient;
        public CreateInternalUserCommandHandler(
             ILogger<CreateInternalUserCommandHandler> logger,
             IIdentityService identityService,
             IMapper mapper, IClientFileFactoryService clientFileFactoryService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _accessManagementApiClient = clientFileFactoryService.GetAccessManagementApiClient();
            _communicationApiClient = clientFileFactoryService.GetCommunicationApiClient();
        }

        public async Task<Result> Handle(CreateInternalUserCommand request, CancellationToken cancellationToken)
        {
            var createUser = request.User;
            string error = await CheckUserExist(createUser.Email, createUser.PhoneNumber, createUser.UserName);
            if (error != "")
            {
                return Result.Failure(error);
            }
            var user = new ApplicationUser
            {
                Email = createUser.Email,
                UserName = createUser.UserName,
                Employee = createUser.FullName,
                PhoneNumberConfirmed = false,
                PhoneNumber = createUser.PhoneNumber,
                NormalizedEmail = createUser.Email.ToUpper(),
                Descriptions = "",
                NormalizedUserName = createUser.UserName.ToUpper(),
                LockoutEnabled = false,
                LockoutEnd = DateTime.Now
            };

            var result = await _identityService.CreateUserAsync(user);
            if (result.Succeeded == false)
            {
                return result;
            }
            var identityResult = await _identityService.AssignUserToRole(user, Roles.InternalUser);
            var sendMailResult = await ConfirmationAccountUsingVerifyLink(user);
            if (sendMailResult.Succeeded == false)
            {
                await _identityService.DeleteUserAsync(user);
                return sendMailResult;
            }
            var createUserProfileResult = await CreateInternalUserProfileAsync(createUser, user);
            if (createUserProfileResult.Succeeded == false)
            {
                await _identityService.DeleteUserAsync(user);
                return Result.Failure(createUserProfileResult.Errors);
            }
            return Result.Success();
        }
        private async Task<string> CheckUserExist(string email, string phoneNumber, string userName)
        {
            Domain.Common.ApplicationUser user = await _identityService.GetUserByUserNameAsync(userName);
            if (user != null)
            {
                return "UserNameIsExist";
            }
            user = await _identityService.GetUserByEmailAsync(email);
            if (user != null)
            {
                return "EmailIsExist";
            }
            user = await _identityService.GetUserByPhoneNumberAsync(phoneNumber);
            if (user != null)
            {
                return "PhoneNumberIsExist";
            }
            return "";
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
        private async Task<Result> CreateInternalUserProfileAsync(CreateInternalUserModel model, ApplicationUser user)
        {
            try
            {
                var userProfile = new AccessManagement.CreateUserProfileModel()
                {
                    UserId = user.Id,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    FullName = model.FullName,
                    BirthDay = null,
                    AccountGroupId = model.AccountGroupId,
                    DepartmentId = model.DepartmentId,
                    Notes = model.Notes
                };

                if (model.Gender != null)
                {
                    userProfile.Gender = (AccessManagement.GenderType)model.Gender;
                }

                var result = await _accessManagementApiClient.ApiAccessmanagementSystemadminCreateinternaluserAsync(userProfile);
                if (result.Succeeded == false)
                {
                    return Result.Failure(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Result.Failure(ex.Message);
            }
            return Result.Success();
        }
    }
}
