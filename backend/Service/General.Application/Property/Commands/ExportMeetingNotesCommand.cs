using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using MediatR;
using General.Domain.Models;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using General.Domain.Models.PropertyElementModels;
using General.Application.Property.Queries;
using Microsoft.AspNetCore.Mvc;
using General.Domain.Constants;
using SelectPdf;
using System.Text.RegularExpressions;
using System.Globalization;

namespace General.Application.Property.Commands
{
    public class ExportMeetingNotesCommand : IRequest<ExportPropertyMeetingNotesModel>
    {
        public Guid PropertyId { set; get; }
    }

    public class ExportMeetingNotesCommandHandler : IRequestHandler<ExportMeetingNotesCommand, ExportPropertyMeetingNotesModel>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IHandlePropertyService _handlePropertyService;
        private readonly IExportTemplateService _exportTemplateService;
        private readonly ICommonFunctionService _commonFunctionService;

        public ExportMeetingNotesCommandHandler(
            IApplicationDbContext dbContext,
            IHandlePropertyService handlePropertyService,
            IMapper mapper,
            IMediator mediator,
            IExportTemplateService exportTemplateService,
            ICommonFunctionService commonFunctionService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
            _exportTemplateService = exportTemplateService ?? throw new ArgumentNullException(nameof(exportTemplateService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<ExportPropertyMeetingNotesModel> Handle(ExportMeetingNotesCommand request, CancellationToken cancellationToken)
        {
            var propertyId = request.PropertyId;
            var vm = new ExportPropertyMeetingNotesModel();

            var property = await _mediator.Send(new GetPropertyByIdQuery() { Id = propertyId, IsAdmin = true });
            var meetingNotes = await _dbContext.PropertyMeetingNote
                .Where(x => x.PropertyId == propertyId)
                .ProjectTo<PropertyMeetingNoteModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            var rawTemplate = await _exportTemplateService.GetExporTemplate(ExportTemplate.MeetingNotesProperty);
            string host = _commonFunctionService.ConvertImageUrl("");
            var body = BuildTemplate(rawTemplate, host, property, meetingNotes);

            HtmlToPdf oHtmlToPdf = new HtmlToPdf();
            oHtmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
            oHtmlToPdf.Options.PageBreaksEnhancedAlgorithm = true;

            oHtmlToPdf.Options.DisplayHeader = true;
            oHtmlToPdf.Header.Height = 75;

            oHtmlToPdf.Options.DisplayFooter = true;
            oHtmlToPdf.Footer.Height = 75;

            PdfDocument oPdfDocument = oHtmlToPdf.ConvertHtmlString(body);
            byte[] pdf = oPdfDocument.Save();
            oPdfDocument.Close();

            vm.Content = pdf;
            vm.ContentType = "application/pdf";
            vm.FileName = $"Meeting_Notes_Property_" + property.PropertyNumber + ".pdf";

            return vm;
        }

        private static string BuildTemplate(string rawTemplate, string host, PropertyModel propertyInfo, PropertyMeetingNoteModel meetingNotes)
        {
            var administractived = (!string.IsNullOrEmpty(propertyInfo.WardName) ? propertyInfo.WardName + ", " : "")
                + propertyInfo.DistrictName + ", " + propertyInfo.ProvinceName;

            string formatted = "";
            if (propertyInfo.Price != null && propertyInfo.Price > 0)
            {
                decimal myDecimal = propertyInfo.Price ?? 0;
                formatted = myDecimal.ToString("N", new CultureInfo("en-US"));
            }

            // Property Summary
            rawTemplate = rawTemplate.Replace("{{Title}}", propertyInfo.Title);
            rawTemplate = rawTemplate.Replace("{{Address}}", administractived);
            rawTemplate = rawTemplate.Replace("{{PropertyAddressEn}}", propertyInfo.PropertyAddressEn);
            rawTemplate = rawTemplate.Replace("{{TransactionTypeEn}}", propertyInfo.TransactionTypeEn);
            rawTemplate = rawTemplate.Replace("{{PropertyTypeEn}}", propertyInfo.PropertyTypeEn);
            rawTemplate = rawTemplate.Replace("{{Price}}", formatted + " " + propertyInfo.CurrencyNotation);
            rawTemplate = rawTemplate.Replace("{{Descriptions}}", propertyInfo.Descriptions);

            // Contact Information
            rawTemplate = rawTemplate.Replace("{{SupplierName}}", propertyInfo.SupplierFirstName + " " + propertyInfo.SupplierLastName);
            rawTemplate = rawTemplate.Replace("{{SupplierPhone}}", propertyInfo.SuppierPhoneNumber1);
            rawTemplate = rawTemplate.Replace("{{SupplierEmail}}", propertyInfo.SupplierEmail);

            // Content Of Meeting
            rawTemplate = rawTemplate.Replace("{{MeetingNoteTitle}}", meetingNotes != null ? meetingNotes.MeetingNoteTitle : "");
            rawTemplate = rawTemplate.Replace("{{MeetingNoteContent}}", meetingNotes != null ? meetingNotes.MeetingNoteContent : "");

            // Orthers
            rawTemplate = rawTemplate.Replace("{{host}}", host);
            rawTemplate = rawTemplate.Replace("{{CurrentDate}}", DateTime.Now.ToString("dd MMM, yyyy", CultureInfo.CreateSpecificCulture("en-US")));

            return rawTemplate;
        }
    }
}
