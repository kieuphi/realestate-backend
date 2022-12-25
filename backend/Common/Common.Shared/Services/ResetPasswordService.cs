using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Shared.Services
{
    public interface IResetPasswordService
    {
        string ResetPasswordUrl { get; }
        public string ResetPasswordInternalUserUrl { get;}
    }

    public class ResetPasswordService : IResetPasswordService
    {
        public ResetPasswordService(string baseUrl, string resetPasswordInternalUserUrl)
        {
            ResetPasswordUrl = baseUrl;
            ResetPasswordInternalUserUrl = resetPasswordInternalUserUrl;
        }
        public string ResetPasswordUrl { get; set; }
        public string ResetPasswordInternalUserUrl { get; set; }
    }
}
