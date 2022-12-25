using Common.Shared.Microservice.IdentityClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Common.Shared.Microservices
{
    public class ClientFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public ClientFactory(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        public IIdentityApiClient GetIdentityApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:IdentityMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer ", "").Replace("bearer ", "");
            if (token != "")
            {
                client.DefaultRequestHeaders.Add("Bearer", token);
            }

            IIdentityApiClient apiClient = new IdentityApiClient(baseUrl, client);
            return apiClient;
        }
    }
}
