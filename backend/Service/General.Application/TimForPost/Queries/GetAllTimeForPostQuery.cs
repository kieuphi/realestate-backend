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

namespace General.Application.TimeForPost.Queries
{
    public class GetAllTimeForPostQuery : IRequest<List<TimeForPostModel>>
    {
    }

    public class GetAllTimeForPostQueryHandler : IRequestHandler<GetAllTimeForPostQuery, List<TimeForPostModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllTimeForPostQueryHandler(
            IMapper mapper,
            IApplicationDbContext context
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<TimeForPostModel>> Handle(GetAllTimeForPostQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.TimeForPost
                               .AsNoTracking()
                               .OrderBy(x => x.Value)
                               .ProjectTo<TimeForPostModel>(_mapper.ConfigurationProvider).ToListAsync();

            return result;
        }
    }
}
