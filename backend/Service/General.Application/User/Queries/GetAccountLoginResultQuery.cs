using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Domain.Common;

namespace General.Application.User.Queries
{
    public class GetUserLoginResultQuery : IRequest<IdentityResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class GetUserLoginResultQueryHandler : IRequestHandler<GetUserLoginResultQuery, IdentityResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserLoginResultQueryHandler> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;

        public GetUserLoginResultQueryHandler(
             ILogger<GetUserLoginResultQueryHandler> logger,
             IIdentityService identityService,
             IJwtAuthManager jwtAuthManager)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(identityService));
            _jwtAuthManager = jwtAuthManager ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<IdentityResult> Handle(GetUserLoginResultQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                var result = await _identityService.SignInAsync(user.UserName, request.Password, request.RememberMe);

                if (user.EmailConfirmed == false)
                {
                    return IdentityResult.LockedOut("EmailNotConfirm");
                }

                if (result.IsLockedOut)
                {
                    return IdentityResult.LockedOut("UserHasBeenLocked");
                }

                if (user == null)
                {
                    return IdentityResult.LockedOut("UserNotExisted");
                }

                if (user.IsDelete == true)
                {
                    return IdentityResult.LockedOut("UserNotExisted");
                }

                if (result.Succeeded)
                {
                    var roles = await _identityService.GetRolesUserAsync(user.UserName);
                    var userId = await _identityService.GetUserIdAsync(user.UserName);
                    await _identityService.ResetAccessFailedCountAsync(userId);

                    List<Claim> claims = BuidUserClaims(user.UserName, userId, roles);

                    var jwtResult = _jwtAuthManager.GenerateTokens(user.UserName, claims.ToArray(), DateTime.Now);

                    _logger.LogInformation($"User [{request.Email}] logged in the system.");

                    await _identityService.SavingUserTokenAsync(user, jwtResult);
                    return IdentityResult.Success(user.UserName, roles, request.Email, jwtResult.AccessToken, jwtResult.RefreshToken.TokenString);
                }
            }

            return IdentityResult.Error("Email or Password incorrect");
        }

        private static List<Claim> BuidUserClaims(string userName, string userId, IList<string> roles)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            return claims;
        }
    }
}
