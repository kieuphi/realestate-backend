using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.Services
{
    public interface ICommonFunctionService
    {
        string ConvertImageUrl(string imageUrl);
    }
    public class CommonFunctionService : ICommonFunctionService
    {
        private readonly IConfiguration _configuration;
        private string _fileUrl = "";
        public CommonFunctionService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fileUrl = _configuration.GetSection("MicroserviceUrl").GetSection("FileMicroserviceUrl").Value;
            if (_fileUrl.EndsWith("/"))
            {
                _fileUrl = _fileUrl.Substring(0, _fileUrl.Length - 1);
            }
        }

        public string ConvertImageUrl(string imageUrl)
        {
            if (imageUrl == null || imageUrl == "")
            {
                return imageUrl;
            }
            if (imageUrl.StartsWith("http"))
            {
                return imageUrl;
            }
            if (imageUrl.StartsWith("/"))
            {
                imageUrl = imageUrl.Substring(1);
            }
            imageUrl = _fileUrl + "/" + imageUrl;
            return imageUrl;
        }


    }
}
