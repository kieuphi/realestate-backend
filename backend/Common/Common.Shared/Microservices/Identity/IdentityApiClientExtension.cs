using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.Microservice.IdentityClient
{
    public partial class IdentityApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IdentityApiClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        
    }
}
