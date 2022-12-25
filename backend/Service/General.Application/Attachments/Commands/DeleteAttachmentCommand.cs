using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Domain.Models;
using Common.Shared.Models;

namespace General.Application.Attachments.Commands
{
    public class DeleteAttachmentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUploadFileService _fileService;

        public DeleteAttachmentCommandHandler(IApplicationDbContext context,
            IUploadFileService fileService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
        {
            var entiy = _context.Attachment.Find(request.Id);

            if (entiy == null)
            {
                throw new ArgumentNullException(nameof(entiy));
            }

            _context.Attachment.Remove(entiy);

            _fileService.DeleteFile(entiy.AttachmentTypeId.ToString(), entiy.FileName);

            return await _context.SaveChangesAsync() > 0 
                ? Result.Success()
                : Result.Failure("Failed to delete attachment");
        }
    }
}
