using General.Application.Interfaces;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Linq;
using General.Domain.Enumerations;
using Common.Shared.Models;

namespace Files.Infrastructure.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<UploadFileService> _logger;
        private readonly IEnvironmentApplication _environmentApplication;

        private const string ErrorMaxFileSize = "File size is too heavy. Please choice another file";
        
        public UploadFileService(IApplicationDbContext dbContext,
            IEnvironmentApplication environmentApplication,
             ILogger<UploadFileService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _environmentApplication = environmentApplication ?? throw new ArgumentNullException(nameof(environmentApplication));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public (Result, string, string) UploadFile(IFormFile file, string domain, string attachmentType, string imageCategory)
        {
            string fileUrl = string.Empty;
            string fileName = string.Empty;

            if (file == null || file.Length == 0)
            {
                return (Result.Failure("File can not be null"), fileUrl, fileName);
            }
            
            string extension = Path.GetExtension(file.FileName);

            if (attachmentType == AttachmentTypes.photo && !AttachmentTypes.GetAllowAttachmentTypes(AttachmentTypes.photo).Contains(extension)
              ||attachmentType == AttachmentTypes.video && !AttachmentTypes.GetAllowAttachmentTypes(AttachmentTypes.video).Contains(extension)
              || attachmentType == AttachmentTypes.excel && !AttachmentTypes.GetAllowAttachmentTypes(AttachmentTypes.excel).Contains(extension)
              || attachmentType == AttachmentTypes.audio && !AttachmentTypes.GetAllowAttachmentTypes(AttachmentTypes.audio).Contains(extension))
            {
                return (Result.Failure("File type not valid"), fileUrl, fileName);
            }

            long fileSizeLimit = AttachmentTypes.GetFileSizeLimitByType(attachmentType);

            if (file.Length > fileSizeLimit)
            {
                return (Result.Failure(ErrorMaxFileSize), fileUrl, fileName);
            }

            try
            {
                string folderToUploads = AttachmentTypes.GetFolderToUploadByType(attachmentType);
                string imageCategoryFolders = ImageCategories.GetFolderToUploadByType(imageCategory);
                string folderName = Path.Combine(_environmentApplication.WebRootPath, folderToUploads + @"\" + imageCategory);
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                fileName = GetFileName(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName);

                string fullPath = Path.Combine(pathToSave, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                fileUrl = $"/{folderToUploads}/{imageCategory}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return (Result.Success(), fileUrl, fileName);
        }

        public void DeleteFile(string attachmentTypeId, string fileName)
        {
            var attachmentTypeEntity = _dbContext.AttachmentType.FirstOrDefault(x => Equals(x.Id, new Guid(attachmentTypeId)));

            if (attachmentTypeEntity == null)
            {
                throw new ArgumentNullException(nameof(attachmentTypeEntity));
            }

            string folderToUploads = AttachmentTypes.GetFolderToUploadByType(attachmentTypeEntity.Name);
            string folderName = Path.Combine(_environmentApplication.WebRootPath, folderToUploads);
            string pathFile = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            string fullPath = Path.Combine(pathFile, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (IOException e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }

        public (Result, string, string) UploadFileWithAttachmentTypeId(IFormFile file, string domain, string attachmentTypeId, string imageCategory)
        {
            var attachmentTypeEntity = _dbContext.AttachmentType.FirstOrDefault(x => Equals(x.Name, attachmentTypeId));

            if (attachmentTypeEntity == null)
            {
                throw new ArgumentNullException(nameof(attachmentTypeEntity));
            }

            return UploadFile(file, domain, attachmentTypeEntity.Name, imageCategory);
        }

        public static string GetRandomString()
        {
            Random random = new Random();

            return DateTime.Now.Day.ToString()
                + "_" + DateTime.Now.Month.ToString()
                + "_" + DateTime.Now.Year.ToString()
                + DateTime.Now.Second.ToString()
                + "_" + random.Next();
        }

        public static string GetFileName(string fileName) => GetRandomString() + fileName.Trim('"');
    }
}
