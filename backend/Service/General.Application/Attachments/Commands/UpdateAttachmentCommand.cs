using AutoMapper;
using MediatR;
using System;
using Models = General.Domain.Models;
using Entities = General.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Domain.Models;
using Common.Exceptions;
using Common.Shared.Models;

namespace General.Application.Attachments.Commands
{
    public class UpdateAttachmentCommand : IRequest<Result>
    {
        public Models.AttachmentModel Entity { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateAttachmentCommandHandler : IRequestHandler<UpdateAttachmentCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateAttachmentCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateAttachmentCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Attachment.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(entity), request.Id);
            }

            entity.FileType = request.Entity.FileType;
            entity.FileSize = request.Entity.FileSize;
            entity.FilePath = request.Entity.FilePath;
            entity.FileName = request.Entity.FileName;
            entity.FileUrl = request.Entity.FileUrl;
            entity.AttachmentTypeId = request.Entity.AttachmentTypeId;

            return await _context.SaveChangesAsync() > 0 ? Result.Success() : Result.Failure("Failed to update attachment");
        }
    }
}
