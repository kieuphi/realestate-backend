using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;
using General.Domain.Models;
using General.Domain.Entities;
using General.Domain.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using General.Application.Common.Interfaces;
using Common.Shared.Enums;
using General.Domain.Common;
using General.Application.Email.Commands;

namespace General.Application.User.Commands
{
    public class AddUserCommand : IRequest<Result>
    {
       public RegisterModel Model { set; get; }
    }

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AddUserCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<ProfileInformationEntity> _repository;
        private readonly IRolesService _rolesService;
        private readonly IMediator _mediator;

        public AddUserCommandHandler(
            ILogger<AddUserCommandHandler> logger,
            IIdentityService identityService,
            IApplicationDbContext context,
            IAsyncRepository<ProfileInformationEntity> repository,
            IRolesService rolesService,
            IMediator mediator)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rolesService = rolesService ?? throw new ArgumentNullException(nameof(rolesService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            string role = _rolesService.GetRoleName(model.Role);
            if (string.IsNullOrEmpty(role))
            {
                return Result.Failure("role is required");
            }
            if (model.Role == RoleTypes.SystemAdministrator)
            {
                return Result.Failure("Role is invalid");
            }

            string error = await CheckUserExist(model.Email, model.UserName);
            if (error != "")
            {
                return Result.Failure(error);
            }

            (Result result, string userId) = await _identityService.CreateUserAsync(model.UserName, model.Email, model.Password, "");
            _logger.LogInformation("Create User done with userId {0}", userId);

            if(result.Succeeded == true)
            {
                var createProfile = await CreateProfile(model, userId);
                if (createProfile.Succeeded == false)
                {
                    var user = await _identityService.GetUserByEmailAsync(model.Email);
                    await _identityService.DeleteUserAsync(user);
                }
            }

            return result;
        }

        private async Task<string> CheckUserExist(string email, string userName)
        {
            ApplicationUser user = await _identityService.GetUserByUserNameAsync(userName);
            if (user != null)
            {
                return "UserNameIsExist";
            }
            user = await _identityService.GetUserByEmailAsync(email);
            if (user != null)
            {
                return "EmailIsExist";
            }
            return "";
        }

        private async Task<Result> ConfirmationAccountUsingVerifyLink(ApplicationUser user, RegisterModel registerModel)
        {
            string token = await _identityService.GenerateEmailConfirmationTokenAsync(user);
            var request = new ConfirmationAccountModel()
            {
                UserName = registerModel.UserName,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                Token = token
            };
            try
            {
                var result = await _mediator.Send(new SendVerifyEmailCommand() { RequestModel = request });
                if (result.Succeeded == false)
                {
                    return Result.Failure(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        private async Task<Result> CreateProfile(RegisterModel model, string userId)
        {
            try
            {
                string role = _rolesService.GetRoleName(model.Role);

                var newId = Guid.NewGuid();
                var user = await _identityService.GetUserByEmailAsync(model.Email);

                var profile = await _context.ProfileInformation.Where(x => x.Id == newId).FirstOrDefaultAsync();
                if (profile != null)
                {
                    return Result.Failure($"The specified Profile is invalid: {newId}");
                }

                var sendMailResult = await ConfirmationAccountUsingVerifyLink(user, model);
                if (sendMailResult.Succeeded == false)
                {
                    await _identityService.DeleteUserAsync(user);
                    return sendMailResult;
                }

                await _identityService.AssignUserToRole(user, role);

                ProfileInformationEntity entity = new ProfileInformationEntity()
                {
                    Id = newId,
                    Avatar = model.Avatar,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber1 = model.PhoneNumber1,
                    PhoneNumber2 = model.PhoneNumber2,
                    PhoneNumber3 = model.PhoneNumber3,
                    TitleVi = model.TitleVi,
                    TitleEn = model.TitleEn,
                    TitleDescriptionVi = model.TitleDescriptionVi,
                    TitleDescriptionEn = model.TitleDescriptionEn,
                    Agency = "",
                    UserId = Guid.Parse(userId),
                    GenderType = model.GenderType,
                    Address = "",
                    License = "",
                    Descriptions = model.Descriptions,
                    Status = ActiveStatus.Active
                };

                CreateSocialNetworkForUser(model, newId);

                await _repository.AddAsync(entity);
                await _context.SaveChangesAsync(new CancellationToken());
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        private void CreateSocialNetworkForUser(RegisterModel model, Guid sellerId)
        {
            if (model.SocialNetworks != null && model.SocialNetworks.Count() > 0)
            {
                foreach (var item in model.SocialNetworks)
                {
                    _context.SocialNetworkUser.Add(new SocialNetworkUserEntity
                    {
                        Id = Guid.NewGuid(),
                        ProfileId = sellerId,
                        SocialNetworkId = item.SocialNetworkId
                    });
                };
            }
        }

    }
}
