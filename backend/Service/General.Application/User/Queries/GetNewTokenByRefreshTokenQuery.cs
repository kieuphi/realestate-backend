using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;

namespace General.Application.User.Queries
{
    public class GetNewTokenByRefreshTokenQuery : IRequest<IdentityResult>
    {
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
    }

    public class GetNewTokenByRefreshTokenQueryHandler : IRequestHandler<GetNewTokenByRefreshTokenQuery, IdentityResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserLoginResultQueryHandler> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;

        public GetNewTokenByRefreshTokenQueryHandler(
             ILogger<GetUserLoginResultQueryHandler> logger,
             IIdentityService identityService,
             IJwtAuthManager jwtAuthManager)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(identityService));
            _jwtAuthManager = jwtAuthManager ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<IdentityResult> Handle(GetNewTokenByRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _identityService.GetUserByUserNameAsync(request.UserName);

                if (user == null)
                {
                    return IdentityResult.Error("Invalid User");
                }

                var validToken = _identityService.GetUserTokenAsync(user, request.RefreshToken);
                if (validToken == false)
                {
                    return IdentityResult.Error("Invalid Token");
                }

                var roles = await _identityService.GetRolesUserAsync(request.UserName);
                var userId = await _identityService.GetUserIdAsync(request.UserName);
                await _identityService.ResetAccessFailedCountAsync(userId);

                List<Claim> claims = BuidUserClaims(request.UserName, userId, roles);

                var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims.ToArray(), DateTime.Now);

                _logger.LogInformation($"User [{user.UserName}] has refreshed JWT token.");

                await _identityService.SavingUserTokenAsync(user, jwtResult);
                return IdentityResult.Success(user.UserName, roles, user.UserName, jwtResult.AccessToken, jwtResult.RefreshToken.TokenString);
            }
            catch (SecurityTokenException e)
            {
                throw e;
            }
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
