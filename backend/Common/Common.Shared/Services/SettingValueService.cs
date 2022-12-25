using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Shared.Services
{
    public interface ISettingValueService
    {
        string VerifyAccountUrl { get; }
    }

    public class SettingValueService : ISettingValueService
    {
        public SettingValueService(string baseUrl)
        {
            VerifyAccountUrl = baseUrl;
        }
        public string VerifyAccountUrl { get; set; }
    }
}
