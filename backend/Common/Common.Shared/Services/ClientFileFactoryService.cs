using Common.Shared.Microservice.CommunicationClient;
using Common.Shared.Microservice.CustomerClient;
using Common.Shared.Microservice.MiligameClient;
using Common.Shared.Microservice.ShippingOrderClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace Common.Shared.Services
{
    public interface IClientFileFactoryService
    {
        Microservice.IdentityClient.IIdentityApiClient GetIdentityApiClient();

        Microservice.AccessManagementClient.IAccessManagementApiClient GetAccessManagementApiClient();
        Microservice.General.IUserManagementApiClient GetUserManagementApiClient();
        Microservice.CustomerClient.ICustomerApiClient GetCustomerApiClient();
        Microservice.CustomerClient.ICustomerApiClient GetCustomerApiClient(string token);
        Microservice.CommunicationClient.ICommunicationApiClient GetCommunicationApiClient();
        Microservice.ShippingOrderClient.IShippingOrderApiClient GetShippingOrderApiClient();

        Microservice.MilixuClient.IMilixuApiClient GetMilixuApiClient();
        Microservice.MiligameClient.IMiligameApiClient GetMiligameApiClient();
        Microservice.MiligameBubbleShootClient.IMiligameBubbleShootApiClient GetMiligameBubbleShootApiClient();

        Microservice.PromotionClient.IPromotionApiClient GetPromotionApiClient();
        Microservice.PromotionClient.IPromotionApiClient GetPromotionApiClient(string token);
    }

    public class ClientFileFactoryService : IClientFileFactoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ClientFileFactoryService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Microservice.IdentityClient.IIdentityApiClient GetIdentityApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:IdentityMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Replace("Bearer ", "").Replace("bearer ", "");
            if (token != "")
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            Microservice.IdentityClient.IIdentityApiClient apiClient = new Microservice.IdentityClient.IdentityApiClient(baseUrl, client);
            return apiClient;
        }

        public Microservice.AccessManagementClient.IAccessManagementApiClient GetAccessManagementApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:AccessManagementMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.AccessManagementClient.IAccessManagementApiClient apiClient = new Microservice.AccessManagementClient.AccessManagementApiClient(baseUrl, client);
            return apiClient;
        }

        public Microservice.General.IUserManagementApiClient GetUserManagementApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:UserManagementMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.General.IUserManagementApiClient apiClient = new Microservice.General.UserManagementApiClient(baseUrl, client);
            return apiClient;
        }

        public Microservice.MilixuClient.IMilixuApiClient GetMilixuApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:MilixuMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.MilixuClient.IMilixuApiClient apiClient = new Microservice.MilixuClient.MilixuApiClient(baseUrl, client);
            return apiClient;
        }

        public Microservice.MiligameBubbleShootClient.IMiligameBubbleShootApiClient GetMiligameBubbleShootApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:MiligameBubbleShootMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.MiligameBubbleShootClient.IMiligameBubbleShootApiClient apiClient = new Microservice.MiligameBubbleShootClient.MiligameBubbleShootApiClient(baseUrl, client);
            return apiClient;
        }

        public ICustomerApiClient GetCustomerApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:CustomerMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.CustomerClient.ICustomerApiClient apiClient = new Microservice.CustomerClient.CustomerApiClient(baseUrl, client);
            return apiClient;
        }
        public ICustomerApiClient GetCustomerApiClient(string token)
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:CustomerMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Microservice.CustomerClient.ICustomerApiClient apiClient = new Microservice.CustomerClient.CustomerApiClient(baseUrl, client);
            return apiClient;
        }

        public Microservice.PromotionClient.IPromotionApiClient GetPromotionApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:PromotionMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.PromotionClient.IPromotionApiClient apiClient = new Microservice.PromotionClient.PromotionApiClient(baseUrl, client);
            return apiClient;
        }
        public Microservice.PromotionClient.IPromotionApiClient GetPromotionApiClient(string token)
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:PromotionMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            Microservice.PromotionClient.IPromotionApiClient apiClient = new Microservice.PromotionClient.PromotionApiClient(baseUrl, client);
            return apiClient;
        }

        public IShippingOrderApiClient GetShippingOrderApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:ShippingOrderMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.ShippingOrderClient.IShippingOrderApiClient apiClient = new Microservice.ShippingOrderClient.ShippingOrderApiClient(baseUrl, client);
            return apiClient;
        }

        public IMiligameApiClient GetMiligameApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:MiligameMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            Microservice.MiligameClient.IMiligameApiClient apiClient = new Microservice.MiligameClient.MiligameApiClient(baseUrl, client);
            return apiClient;
        }

        public ICommunicationApiClient GetCommunicationApiClient()
        {
            string baseUrl = _configuration.GetSection("MicroserviceUrl:CommunicationMicroserviceUrl").Value;
            HttpClient client = new HttpClient();
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                token = token.Replace("Bearer ", "").Replace("bearer ", "");
                if (token != "")
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }

            ICommunicationApiClient apiClient = new Microservice.CommunicationClient.CommunicationApiClient(baseUrl, client);
            return apiClient;
        }
    }
}