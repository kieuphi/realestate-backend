using MediatR;
using System;
using System.Collections.Generic;
using Models = General.Domain.Models;
using System.Threading.Tasks;
using General.Application.Interfaces;
using AutoMapper;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using General.Domain.Enumerations;

namespace General.Application.Attachments.Queries
{
    public class GetAttachmentsByIdQuery : IRequest<List<Models.AttachmentModel>>
    {
        public string Type { get; set; }
        public Guid FileId { get; set; }
    }

    public class GetAttachmentsByIdQueryHandler : IRequestHandler<GetAttachmentsByIdQuery, List<Models.AttachmentModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAttachmentsByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<Models.AttachmentModel>> Handle(GetAttachmentsByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Attachment
                .Where(x=>x.Id == request.FileId)
                .Include(x => x.AttachmentType)
                .OrderByDescending(x => x.CreateTime)
                .AsQueryable();

            if (string.Equals(request.Type, AttachmentTypes.photo))
            {
                query = query.Where(e => string.Equals(e.AttachmentType.Name, AttachmentTypes.photo));
            }

            if (string.Equals(request.Type, AttachmentTypes.video))
            {
                query = query.Where(e => string.Equals(e.AttachmentType.Name, AttachmentTypes.video));
            }

            if (string.Equals(request.Type, AttachmentTypes.audio))
            {
                query = query.Where(e => string.Equals(e.AttachmentType.Name, AttachmentTypes.audio));
            }

            var result = await query.AsNoTracking().ToListAsync();

            return await Task.FromResult(_mapper.Map<List<Models.AttachmentModel>>(result));
        }
    }
}
