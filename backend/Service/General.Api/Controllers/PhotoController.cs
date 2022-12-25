using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using General.Domain.Models;
using System.IO;
using General.Application.Attachments.Queries;
using General.Application.Attachments.Commands;
using System.Collections.Generic;
using General.Domain.Enumerations;
using General.Application.Interfaces;
using Common.Shared.Models;

namespace General.Api.Controllers
{
    public class PhotoController : ApiController
    {
        private readonly ILogger<PhotoController> _logger;
        private readonly IUploadFileService _uploadService;

        public PhotoController(IMediator mediator,
            IUploadFileService uploadService,
            ILogger<PhotoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uploadService = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
        }

        [HttpGet("multiple")]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AttachmentModel>>> GetAllPhotoAttachments(string attachmentType)
        {
            if (string.IsNullOrWhiteSpace(attachmentType) || !attachmentType.Equals(AttachmentTypes.photo))
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new GetAllAttachmentsQuery() { Type = AttachmentTypes.photo });
            return Ok(result);
        }

        [HttpPost("GetImagesByFilter")]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AttachmentModel>>> GetImagesByFilter(AttachmentCollectionFilterModel attachmentCollectionFilter)
        {
            if (!attachmentCollectionFilter.AttachmentType.Equals(AttachmentTypes.photo))
            {
                return BadRequest(attachmentCollectionFilter);
            }

            var result = await Mediator.Send(new GetAttachmentsFilterQuery() { AttachmentCollectionFilter = attachmentCollectionFilter });
            return result;
        }

        [HttpGet("single/{fileId}")]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(List<AttachmentModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AttachmentModel>>> GetPhotoByFileId(Guid fileId, string attachmentType)
        {
            if (string.IsNullOrWhiteSpace(attachmentType) || !attachmentType.Equals(AttachmentTypes.photo) || fileId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetAttachmentsByIdQuery() { Type = AttachmentTypes.photo, FileId = fileId });

            foreach (var item in result)
            {
                item.FilePath = GetDomain() + $"/{item.FilePath}";
            }

            return Ok(result);
        }

        [HttpDelete("single/{fileId}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> DeletePhotoById(Guid fileId, string attachmentType)
        {
            if (string.IsNullOrWhiteSpace(attachmentType) || !attachmentType.Equals(AttachmentTypes.photo) || fileId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new DeleteAttachmentCommand() { Id = fileId });

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("single"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UploadMultipleFileRequestModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> PostPhotoAttachment([FromForm] UploadFileRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            if (request.AttachmentType == null || !request.AttachmentType.Equals(AttachmentTypes.photo))
            {
                return BadRequest();
            }

            if (request.File == null)
            {
                return BadRequest(request);
            }

            (Result resultUpload, string fileUrl, string fileName) = _uploadService
                    .UploadFileWithAttachmentTypeId(request.File,
                                GetDomain(),
                                AttachmentTypes.photo,
                                request.ImageCategory
                             );

            if (!resultUpload.Succeeded)
            {
                return BadRequest(resultUpload.Errors);
            }

            AttachmentModel attachment = InitAttachment(request.File, fileUrl, fileName);
            _logger.LogInformation("Upload result", resultUpload);

            if (request.ReferenceId != null)
            {
                attachment.ReferenceId = request.ReferenceId;
            }

            var result = await Mediator.Send(new AddAttachmentCommand() { Model = attachment , AttachmentType = request.AttachmentType, ImageCategory = request.ImageCategory });
            return Ok(result);
        }


        [HttpPost("multiple"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> PostMultiplePhotoAttachment([FromForm] UploadMultipleFileRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(request);
            }

            if (request.AttachmentType == null || !request.AttachmentType.Equals(AttachmentTypes.photo))
            {
                return BadRequest();
            }

            if (request.Files == null)
            {
                return BadRequest();
            }

            foreach (var item in request.Files)
            {
                if (item == null)
                {
                    return BadRequest(request);
                }
            }

            var results = new List<object>(); 
            foreach (var item in request.Files)
            {
                (Result resultUpload, string fileUrl, string fileName) = _uploadService
                        .UploadFile(item, GetDomain(), AttachmentTypes.photo, request.ImageCategory);

                if (resultUpload.Succeeded)
                {
                    AttachmentModel attachment = InitAttachment(item, fileUrl, fileName);

                    if (request.ReferenceId != null)
                    {
                        attachment.ReferenceId = request.ReferenceId;
                    }

                    _logger.LogInformation("Upload result", resultUpload);
                    var result = await Mediator.Send(new AddAttachmentCommand() { Model = attachment, AttachmentType = request.AttachmentType, ImageCategory = request.ImageCategory });
                    results.Add(result.ObjectReturn);
                }
            }

            return Ok(Result.Success(results));
        }

        private static AttachmentModel InitAttachment(IFormFile file, string fileUrl, string fileName)
        {
            return new AttachmentModel
            {
                FileName = fileName,
                FileSize = file.Length,
                FileType = Path.GetExtension(file.FileName),
                FileUrl = fileUrl,
                FilePath = fileUrl
            };
        }
    }
}
