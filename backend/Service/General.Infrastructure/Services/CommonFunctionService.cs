using General.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace General.Infrastructure.Services
{
    public class CommonFunctionService : ICommonFunctionService
    {
        private readonly IConfiguration _configuration;
        private readonly IConvertVietNameseService _convertVietNameseService;
        private string _fileUrl = "";

        public CommonFunctionService(
            IConfiguration configuration,
            IConvertVietNameseService convertVietNameseService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
            _fileUrl = _configuration.GetSection("ServiceUrl").GetSection("Domain").Value;
            if (_fileUrl.EndsWith("/"))
            {
                _fileUrl = _fileUrl.Substring(0, _fileUrl.Length - 1);
            }
        }

        public string ConvertImageUrl(string imageUrl)
        {
            if(imageUrl == null || imageUrl == "")
            {
                return _fileUrl;
            }
            if (imageUrl.StartsWith("/"))
            {
                imageUrl = imageUrl.Substring(1);
            }
            imageUrl = _fileUrl + "/" + imageUrl;
            return imageUrl;
        }

        public string GenerateFriendlyUrl(string keyword, int count)
        {
            string result = "";
            result = _convertVietNameseService.ConvertVietNamese(keyword.ToLower());
            result = Regex.Replace(result, @"[^0-9a-zA-Z]+", " ");
            result = result.Trim();
            result = Regex.Replace(result, @"\s+", "-");

            string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() + count.ToString();
            result = !string.IsNullOrEmpty(result) ? (result + "-" + timestamp) : timestamp;

            return result;
        }
    }
}
