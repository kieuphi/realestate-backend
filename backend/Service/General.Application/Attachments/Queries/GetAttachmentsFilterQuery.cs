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
using General.Domain.Models;

namespace General.Application.Attachments.Queries
{
    public class GetAttachmentsFilterQuery : IRequest<List<Models.AttachmentModel>>
    {
        public AttachmentCollectionFilterModel AttachmentCollectionFilter { get; set; }
    }

    public class GetAttachmentsFilterQueryHandler : IRequestHandler<GetAttachmentsFilterQuery, List<Models.AttachmentModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAttachmentsFilterQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<Models.AttachmentModel>> Handle(GetAttachmentsFilterQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Attachment
                .Where(x =>x.ReferenceId != null)
                .Include(x => x.AttachmentType)
                .AsQueryable();

            query = query.Where(e => string.Equals(e.AttachmentType.Name, AttachmentTypes.photo));

            if (request.AttachmentCollectionFilter.ReferenceIds != null && request.AttachmentCollectionFilter.ReferenceIds.Count > 0)
            {
                query = query.Where(x => request.AttachmentCollectionFilter.ReferenceIds.Contains(x.ReferenceId));
            }

            var attachments = await query.AsNoTracking()
                .OrderBy(x => x.AttachmentType.Name)
                .ThenByDescending(x =>x.CreateTime).ToListAsync();

            var result = _mapper.Map<List<Models.AttachmentModel>>(attachments);
            return result;
        }
    }
}
