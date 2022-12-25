using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Shared.Microservices
{
    public class BaseClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _baseUrl;
        public BaseClient(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        }

        protected async Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
        {
            HttpClient client = new HttpClient();
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer ", "").Replace("bearer ", "");
            if (token != "")
            {
                client.DefaultRequestHeaders.Add("Bearer", token);
            }
            return client;
        }
    }
}
