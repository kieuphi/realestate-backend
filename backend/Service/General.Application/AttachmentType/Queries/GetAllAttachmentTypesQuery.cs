using MediatR;
using System;
using System.Collections.Generic;
using Models = General.Domain.Models;
using System.Threading.Tasks;
using General.Application.Interfaces;
using AutoMapper;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace General.Application.AttachmentType.Queries
{
    public class GetAllAttachmentTypesQuery : IRequest<List<Models.AttachmentTypeModel>>
    {
    }

    public class GetAllAttachmentTypesQueryHandler : IRequestHandler<GetAllAttachmentTypesQuery, List<Models.AttachmentTypeModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllAttachmentTypesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<Models.AttachmentTypeModel>> Handle(GetAllAttachmentTypesQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.AttachmentType.AsNoTracking().ToListAsync();
            return await Task.FromResult(_mapper.Map<List<Models.AttachmentTypeModel>>(result));
        }
    }
}
