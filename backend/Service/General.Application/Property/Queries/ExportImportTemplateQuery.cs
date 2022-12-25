using AutoMapper;
using General.Application.Interfaces;
using General.Domain.Common.Excel;
using General.Domain.Models;
using MediatR;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Property.Queries
{
    public class ExportImportTemplateQuery : IRequest<ExportTemplatePropertyModel>
    {
    }

    public class ExportImportTemplateQueryHandler : IRequestHandler<ExportImportTemplateQuery, ExportTemplatePropertyModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IEnvironmentApplication _environmentApplication;
        protected internal readonly ExcelPackage Package;

        public ExportImportTemplateQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IEnvironmentApplication environmentApplication
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _environmentApplication = environmentApplication ?? throw new ArgumentNullException(nameof(environmentApplication));
            Package = new ExcelPackage();
        }

        public async Task<ExportTemplatePropertyModel> Handle(ExportImportTemplateQuery request, CancellationToken cancellationToken)
        {
            using (var stream = new FileStream(_environmentApplication.WebRootPath + ($"/importTemplate/Import_Property_Template.xlsx"), FileMode.Open, FileAccess.ReadWrite))
            {
                var vm = new ExportTemplatePropertyModel();
                Package.Load(stream);

                //var excel = new ExcelWriter(fileName: $"TimeKeepingReport-" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx");
                //ExcelSheet excelSheet = excel.CreateWorksheet("Timekeeping Report");

                // result
                vm.Content = Package.GetAsByteArray();
                vm.ContentType = ExcelWriter.ContentType;
                vm.FileName = "Import_Property_Template.xlsx";

                return await Task.FromResult(vm);
            }
        }
    }
}
