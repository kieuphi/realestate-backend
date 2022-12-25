using General.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;

namespace General.Api.Configs
{
    public class EnvironmentApplication : IEnvironmentApplication
    {
        private readonly IWebHostEnvironment _environment;
        public EnvironmentApplication(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public string WebRootPath => _environment.WebRootPath;
        public string EnvironmentName => _environment.EnvironmentName;
    }
}
