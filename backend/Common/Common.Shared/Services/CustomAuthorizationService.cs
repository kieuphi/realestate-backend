using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microservice = Common.Shared.Microservice;

namespace Common.Shared.Services
{
    public interface ICustomAuthorizationService
    {
        Task<bool> CheckTokenValidAtAccessManagement();
        Task<bool> CheckTokenValidAtAccessManagementWithRole(string role);
        Task<string> GetTokenLocalService();
    }
    public class CustomAuthorizationService : ICustomAuthorizationService
    {
        private readonly IClientFileFactoryService _clientFileFactoryService;
        private readonly ILogger<CustomAuthorizationService> _logger;
        public CustomAuthorizationService(IClientFileFactoryService clientFileFactoryService, ILogger<CustomAuthorizationService> logger)
        {
            _clientFileFactoryService = clientFileFactoryService ?? throw new ArgumentNullException(nameof(clientFileFactoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> CheckTokenValidAtAccessManagement()
        {
            try
            {
                var apiClient = _clientFileFactoryService.GetAccessManagementApiClient();
                await apiClient.ApiAccessmanagementUserprofileCheckauthorizeAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
            return true;
        }

        public async Task<bool> CheckTokenValidAtAccessManagementWithRole(string role)
        {
            try
            {
                var apiClient = _clientFileFactoryService.GetAccessManagementApiClient();
                if (role == "SystemAdministrator")
                {
                    await apiClient.ApiAccessmanagementUserprofileCheckauthorizesystemadministratorAsync();
                }
                else if (role == "LocalService")
                {
                    await apiClient.ApiAccessmanagementUserprofileCheckauthorizelocalserviceAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
            return true;
        }
        
        public async Task<string> GetTokenLocalService()
        {
            var apiClient = _clientFileFactoryService.GetUserManagementApiClient();
            Microservice.General.LoginUserModel loginData = new Microservice.General.LoginUserModel
            {
                UserName = "localservice",
                Password = "Localservice@123",
                RememberMe = false
            };

            var result = await apiClient.ApiUserUserLoginAsync(loginData);
            string token = "";
            if (result.Succeeded == true)
            {
                token = result.AccessToken;
            }
            return token;
        }
    }
}
