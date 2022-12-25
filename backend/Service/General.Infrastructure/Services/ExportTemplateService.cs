using General.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace General.Infrastructure.Services
{
    public class ExportTemplateService : IExportTemplateService
    {
        private readonly IEnvironmentApplication _environmentApplication;

        public ExportTemplateService(IEnvironmentApplication environmentApplication)
        {
            _environmentApplication = environmentApplication ?? throw new ArgumentNullException(nameof(environmentApplication));
        }

        public async Task<string> GetExporTemplate(string templateName)
        {
            string templateEmail = string.Empty;
            using (StreamReader reader = new StreamReader(_environmentApplication.WebRootPath + ($"/exportTemplates/{templateName}.html")))
            {
                templateEmail = await reader.ReadToEndAsync();
            }

            return templateEmail;
        }
    }
}
