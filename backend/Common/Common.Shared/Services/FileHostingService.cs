using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Shared.Services
{
    public interface IFileHostingService
    {
        string FileHostingDomain { get; }
    }

    public class FileHostingService : IFileHostingService
    {
        public FileHostingService(string baseUrl)
        {
            FileHostingDomain = baseUrl;
        }
        public string FileHostingDomain { get; set; }
    }
}
