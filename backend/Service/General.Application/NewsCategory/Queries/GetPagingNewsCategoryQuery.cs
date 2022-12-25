using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
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
    public class GetPagingNewsCategoryQuery : IRequest<PaginatedList<ListNewsCategoryModel>>
    {
        public PagingNewsCategoryModel PagingModel { set; get; }
    }

    public class GetPagingNewsCategoryQueryHandler : IRequestHandler<GetPagingNewsCategoryQuery, PaginatedList<ListNewsCategoryModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetPagingNewsCategoryQueryHandler(
            IMapper mapper,
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginatedList<ListNewsCategoryModel>> Handle(GetPagingNewsCategoryQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            var newsCategory = await _context.NewsCategory
                .Where(x => x.IsDeleted == DeletedStatus.False 
                    && x.IsApprove == NewsApproveStatus.Active)
                .AsNoTracking()
                .OrderByDescending(x => x.CreateTime)
                .ProjectTo<ListNewsCategoryModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<ListNewsCategoryModel>(newsCategory, newsCategory.Count, 1, newsCategory.Count);
            }

            var paginatedList = PaginatedList<ListNewsCategoryModel>.Create(newsCategory, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
