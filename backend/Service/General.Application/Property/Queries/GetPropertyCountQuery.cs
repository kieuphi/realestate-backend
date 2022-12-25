using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Property.Queries
{
    public class GetPropertyCountQuery : IRequest<int>
    {
    }

    public class GetPropertyCountQueryHandler : IRequestHandler<GetPropertyCountQuery, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPropertyCountQueryHandler(
            IApplicationDbContext context,
            IMapper mapper
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> Handle(GetPropertyCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Property
                                .Where(x => x.IsDeleted == DeletedStatus.False)
                                .AsNoTracking()
                                .CountAsync();

            return result;
        }
    }
}
