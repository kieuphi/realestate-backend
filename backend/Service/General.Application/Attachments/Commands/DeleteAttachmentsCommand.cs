using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Domain.Models;
using System.Collections.Generic;
using General.Domain.Entities;
using Common.Shared.Models;

namespace General.Application.Attachments.Commands
{
    public class DeleteAttachmentsCommand : IRequest<Result>
    {
        public List<string> Items { get; set; }
    }

    public class DeleteAttachmentsCommandHandler : IRequestHandler<DeleteAttachmentsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUploadFileService _fileService;
        private readonly IMapper _mapper;

        public DeleteAttachmentsCommandHandler(IApplicationDbContext context,
            IUploadFileService fileService,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _fileService = fileService;
        }

        public async Task<Result> Handle(DeleteAttachmentsCommand request, CancellationToken cancellationToken)
        {
            var entites = new List<AttachmentEntity>();
            
            foreach (var item in request.Items)
            {
                var entity = _context.Attachment.Find(new Guid(item));
                entites.Add(entity);
            }

            _context.Attachment.RemoveRange(entites);

            foreach (var item in entites)
            {
                _fileService.DeleteFile(item.AttachmentTypeId.ToString(), item.FileName);
            }

            return await _context.SaveChangesAsync() > 0 
                ? Result.Success()
                : Result.Failure("Failed to delete attachment");
        }
    }
}
