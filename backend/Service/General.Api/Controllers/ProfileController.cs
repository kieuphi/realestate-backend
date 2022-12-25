using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Application.Profile.Commands;
using General.Application.Profile.Queries;
using General.Domain.Models;
using Common.Shared.Models;

namespace General.Api.Controllers
{
    //PRODUCTION EXPECTED
    //[Authorize]
    public class ProfileController : ApiController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(ILogger<ProfileController> logger,
            ICurrentUserService currentUserService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ChangePasswordModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> ChangePassword([FromBody] ChangePasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            if (request.OldPassword.Equals(request.NewPassword))
            {
                _logger.LogInformation("The new password must be different with old password");
                return Ok(Result.Failure("TheNewPasswordMustBeDifferentWithOldPassword"));
            }


            var result = await Mediator.Send(new UpdatePasswordCommand
            {
                UserName = request.UserName,
                CurrentUserId = _currentUserService.UserId,
                NewPassword = request.NewPassword,
                OldPassword = request.OldPassword
            });
            return result;
            /*
            if (result.Succeeded)
            {
                _logger.LogInformation("User changed their password successfully.");
                return Ok(Result.Success());
            }

            return BadRequest(result);*/
        }

        [Authorize]
        [HttpPut("update-profile")]
        [ProducesResponseType(typeof(UpdateProfileModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> UpdateProfile([FromBody] UpdateProfileModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            var result = await Mediator.Send(new UpdateProfileCommand{PersonalInformation = request});

            if (result.Succeeded)
            {
                _logger.LogInformation("User changed their information successfully.");
                return Ok(Result.Success());
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("forget-password")]
        [ProducesResponseType(typeof(ForgetPasswordModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> ForgetPassword([FromBody] ForgetPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            var result = await Mediator.Send(new CreateNewPasswordCommand
            {
                Email = request.Email
            });

            _logger.LogInformation("User reset their password successfully.");
            return Ok(result);
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResult>> UserInfo(string userName)
        {
            var result = await Mediator.Send(new GetProfileUserByUserNameQuery
            {
                UserName = userName
            });

            if (result == null)
            {
                return NotFound(Result.Failure("User is invalid"));
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("info")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResult>> UserInfo()
        {
            if (string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return NotFound(Result.Failure("User were logout"));
            }

            var result = await Mediator.Send(new GetInfomationsUserQuery
            {
                UserId = _currentUserService.UserId
            });

            if (result == null)
            {
                return NotFound(Result.Failure("User were logout"));
            }

            return Ok(result);
        }
    }
}
