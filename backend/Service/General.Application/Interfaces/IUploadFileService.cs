using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Common.Shared.Models;

namespace General.Application.Interfaces
{
    public interface IUploadFileService
    {
        (Result, string, string) UploadFile(IFormFile file, string domain, string attachmentTypes, string imageCategory);
        (Result, string, string) UploadFileWithAttachmentTypeId(IFormFile file, string domain, string attachmentTypeId, string imageCategory);
        void DeleteFile(string attachmentTypeId, string fileName);
    }
}
