using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Application.ManageUser.Commands;
using General.Application.ManageUser.Queries;
using General.Domain.Enums;
using General.Domain.Models;
using Common.Shared.Models;

namespace General.Api.Controllers
{
    //PRODUCTION EXPECTED
    [Authorize(Roles = Roles.SystemAdministrator)]
    public class AdminController : ApiController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AdminController(ILogger<AdminController> logger,
            ICurrentUserService currentUserService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        [HttpGet]
        public IActionResult CheckAdminCanAccess()
        {
            _logger.LogInformation("Admin access successfully");
            return Ok();
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(List<UserResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserResult>>> RetriveAllUsersAsync()
        {
            var users = await Mediator.Send(new GetAllUserQuery { });
            users.RemoveAll(i => i.Id == _currentUserService.UserId);
            return Ok(users);
        }

        [HttpGet]
        [Route("GetAllInternalAccount")]
        [ProducesResponseType(typeof(List<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<UserModel>>> GetAllInternalAccount()
        {
            var data = await Mediator.Send(new GetAllInternalUserQuery());

            return data;
        }

        [HttpGet("GetByUserId")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserModel>> GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            var data = await Mediator.Send(new GetUserByUserIdQuery()
            {
                UserId = userId
            });

            return data;
        }

        [HttpPost("users")]
        [ProducesResponseType(typeof(List<CreateUserResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<CreateUserResult>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CreateUserResult>>> CreateUserAsync([FromBody] List<CreateUserModel> createUsersRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(createUsersRequest);
            }

            return Ok(await Mediator.Send(new CreateUsersCommand { Users = createUsersRequest }));
        }

        [HttpPost]
        [Route("CreateInternalUser")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> CreateInternalUser(CreateInternalUserModel createUserProfile)
        {
            if (createUserProfile == null) return BadRequest();

            Result result = await Mediator.Send(new CreateInternalUserCommand()
            {
                User = createUserProfile
            });
            result = LocalizationParser.ConvertResult(result);
            return Ok(result);
        }

        [HttpPost]
        [Route("ResendConfirmationAccountToken")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateInternalUserModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> ResendConfirmationAccountTokenAsync([FromBody] string userId)
        {
            Result result = await Mediator.Send(new ResendConfirmationAccountTokenCommand()
            {
                UserId = userId
            });
            result = LocalizationParser.ConvertResult(result);
            return Ok(result);
        }

        [HttpPost("users/reset-password")]
        [ProducesResponseType(typeof(List<ResetPasswordResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ResetPasswordResult>>> ResetPasswordUsersAsync([FromBody] List<string> userEmails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(userEmails);
            }

            return Ok(await Mediator.Send(new ResetUsersPasswordCommand { UserEmails = userEmails }));
        }

        [HttpPost("users/lock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> LockUsersAsync([FromBody] List<string> users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(users);
            }

            return Ok(await Mediator.Send(new LockUsersCommand { Users = users }));
        }

        [HttpPost("user/lock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> LockUserAsync([FromBody] LockUserModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            Result result = await Mediator.Send(new LockUserCommand {
                UserId = request.UserId
            });
       
            return Ok(result);
        }

        [HttpPost("user/unlock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> UnlockUserAsync([FromBody] LockUserModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            Result result = await Mediator.Send(new UnlockUserCommand
            {
                UserId = request.UserId
            });

            return Ok(result);
        }

        [HttpPut("UpdateInternalUser")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> UpdateInternalUser([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(user);
            }

            Result result = await Mediator.Send(new UpdateInternalUserCommand()
            {
                User= user
            });

            return Ok(result);
        }

        [HttpGet("roles")]
        [ProducesResponseType(typeof(List<RoleModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RoleModel>>> GetRolesAsync()
        {
            var roles = await Mediator.Send(new GetRolesQuery { });
            roles.RemoveAll(i => i.Name.Equals(Roles.SystemAdministrator));
            return Ok(roles);
        }

        [HttpGet("user/verify/{email}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result>> VerifyUserAsync(string email)
        {
            var users = await Mediator.Send(new GetAllUserQuery { });

            var isExist = users.Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (isExist)
            {
                return Ok(Result.Failure("User already issued"));
            }

            return Ok(Result.Success());
        }
    }
}
