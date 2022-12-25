using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Application.User.Commands;
using General.Application.User.Queries;
using System.Security.Claims;
using System.Linq;
using System;
using General.Domain.Models;
using Common.Shared.Models;
using General.Application.ManageUser.Commands;

namespace General.Api.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UserController(ILogger<UserController> logger, 
            ICurrentUserService currentUserService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status402PaymentRequired)]
        public async Task<ActionResult<IdentityResult>> Login([FromBody] LoginUserModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            IdentityResult result = await Mediator.Send(new GetUserLoginResultQuery
            {
                Email = request.Email,
                Password = request.Password,
                RememberMe = request.RememberMe,
            });
            result.ErrorMessage = LocalizationParser.ConvertText(result.ErrorMessage);
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Logout()
        {
            var userName = User.Identity.Name;

            await Mediator.Send(new DeleteRefreshTokenCommand
            {
                UserName = userName
            });

            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IdentityResult>> RefreshToken([FromBody] RefreshTokenModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                IdentityResult result = await Mediator.Send(new GetNewTokenByRefreshTokenQuery
                {
                    RefreshToken = request.RefreshToken,
                    UserName = request.UserName
                });

                return Ok(result);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        [HttpGet("info")]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public ActionResult GetUserInfo()
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new IdentityResult
            {
                UserName = User.Identity.Name,
                Roles = roles,
                Email = User.FindFirst("Email")?.Value
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyAccount")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> VerifyAccount([FromBody] VerifyAccountModel request)
        {
            if (request == null) return BadRequest();

            Result result = await Mediator.Send(new VerifyInternalUserCommand()
            {
                VerifyAccountModel = request
            });

            return Ok(result);
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

            var result = await Mediator.Send(new ForgetPasswordCommand
            {
                Email = request.Email
            });
            result = LocalizationParser.ConvertResult(result);
            _logger.LogInformation("User reset their password successfully.");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> ResetPassword([FromBody] ResetPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            var result = await Mediator.Send(new ResetNewPasswordCommand
            {
                ResetPasswordModel = request
            });
            result = LocalizationParser.ConvertResult(result);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Register([FromBody] RegisterModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            Result result = await Mediator.Send(new AddUserCommand
            {
                Model = request
            });

            if (result.Succeeded)
            {
                _logger.LogInformation($"User [{request.Email}] register successfully.");

                return Ok(result);
            }

            //if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid userId)
        {
            if (userId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteUserCommand()
            {
                UserId = userId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}
