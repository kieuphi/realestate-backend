using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Shared.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string UserName { get; }
        string PhoneNumber { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public string PhoneNumber => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.MobilePhone);
    }
}
