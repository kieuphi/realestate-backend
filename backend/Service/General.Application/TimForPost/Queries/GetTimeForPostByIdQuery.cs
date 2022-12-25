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
    public class GetTimeForPostByIdQuery : IRequest<TimeForPostModel>
    {
        public Guid Id { set; get; }
    }

    public class GetTimeForPostByIdQueryHandler : IRequestHandler<GetTimeForPostByIdQuery, TimeForPostModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTimeForPostByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TimeForPostModel> Handle(GetTimeForPostByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.TimeForPost
                            .Where(x => x.Id == request.Id)
                            .AsNoTracking()
                            .ProjectTo<TimeForPostModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            return result;
        }
    }
}
