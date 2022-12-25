using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.NewsCategory.Queries
{
    public class GetAllNewsCategoryQuery : IRequest<List<ListNewsCategoryModel>>
    {
    }

    public class GetAllNewsCategoryQueryHandler : IRequestHandler<GetAllNewsCategoryQuery, List<ListNewsCategoryModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllNewsCategoryQueryHandler(
            IMapper mapper,
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ListNewsCategoryModel>> Handle(GetAllNewsCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.NewsCategory
                .Where(x => x.IsDeleted == DeletedStatus.False &&
                    x.IsApprove == NewsApproveStatus.Active)
                .AsNoTracking()
                .OrderByDescending(x => x.CreateTime)
                .ProjectTo<ListNewsCategoryModel>(_mapper.ConfigurationProvider).ToListAsync();

            return result;
        }
    }
}
