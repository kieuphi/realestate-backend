using General.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Services
{
    public class SettingValueService : ISettingValueService
    {
        public SettingValueService(string baseUrl)
        {
            VerifyAccountUrl = baseUrl;
        }

        public string VerifyAccountUrl { get; set; }
    }
}
