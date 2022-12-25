using AutoMapper;
using General.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.PropertyHeart.Queries
{
    public class IsManyPropertyHeartQuery : IRequest<List<bool>>
    {
        public Guid UserId { set; get; }
        public List<Guid> PropertyIds { set; get; }
    }

    public class IsManyPropertyHeartQueryHandler : IRequestHandler<IsManyPropertyHeartQuery, List<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public IsManyPropertyHeartQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<bool>> Handle(IsManyPropertyHeartQuery request, CancellationToken cancellationToken)
        {
            var propertyIds = request.PropertyIds;
            List<bool> results = new List<bool>();

            var entity = await _context.PropertyHeart
                .Where(X => X.UserId == request.UserId)
                .ToListAsync();

            for (int i = 0; i < propertyIds.Count(); i++)
            {
                var heartExisted = entity.Where(x => x.PropertyId == propertyIds[i]).FirstOrDefault();
                if (heartExisted != null)
                {
                    results.Add(true);
                } else
                {
                    results.Add(false);
                }
            }

            return results;
        }
    }
}
