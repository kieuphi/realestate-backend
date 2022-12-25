using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;

namespace Common.Shared.Microservice.AccessManagementClient
{
    partial class AccessManagementApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AccessManagementApiClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : this(configuration.GetSection("MicroserviceUrl:AccessManagementMicroserviceUrl").Value, new HttpClient())
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /*
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "Authorization");
            request.Headers.Add("Authorization", authorizationHeader.Value.ToString());
        }*/
    }
}