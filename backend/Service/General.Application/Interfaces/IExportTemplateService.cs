using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace General.Application.Interfaces
{
    public interface IExportTemplateService
    {
        Task<string> GetExporTemplate(string templateName);
    }
}
