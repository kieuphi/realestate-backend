using General.Domain.Enumerations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace General.Domain.Models
{
    public class UploadFileRequestModel
    {
        public IFormFile File { get; set; }
        public string AttachmentType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string ImageCategory { set; get; }
    }

    public class UploadMultipleFileRequestModel
    {
        public List<IFormFile> Files { get; set; }
        public string AttachmentType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string ImageCategory { set; get; }
    }
}
