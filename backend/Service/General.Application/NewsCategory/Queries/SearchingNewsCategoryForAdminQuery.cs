using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using Common.Shared.Models;
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
    public class SearchingNewsCategoryForAdminQuery : IRequest<PaginatedList<ListNewsCategoryModel>>
    {
        public SearchingNewsCategoryModel Model { set; get; }
    }

    public class SearchingNewsCategoryForAdminQueryHandler : IRequestHandler<SearchingNewsCategoryForAdminQuery, PaginatedList<ListNewsCategoryModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public SearchingNewsCategoryForAdminQueryHandler(IMapper mapper, IApplicationDbContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginatedList<ListNewsCategoryModel>> Handle(SearchingNewsCategoryForAdminQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var queryData = _context.NewsCategory.AsNoTracking();

            if (!string.IsNullOrEmpty(model.Title))
            {
                var title = model.Title.ToLower().Trim();
                queryData = queryData.Where(x => x.CategoryNameVi.ToLower().Contains(title) || x.CategoryNameEn.ToLower().Contains(title));
            }

            if (model.IsApprove > 0 && model.IsApprove != null)
            {
                queryData = queryData.Where(p => p.IsApprove == model.IsApprove);
            }
            else if (model.IsApprove == NewsApproveStatus.Lock)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.True && x.IsApprove == NewsApproveStatus.Lock);
            }
            else if (model.IsApprove == 0 || model.IsApprove == null)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.False);
            }

            var finalQuery = await queryData
                    .OrderByDescending(x => x.CreateTime)
                    .ProjectTo<ListNewsCategoryModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = finalQuery.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListNewsCategoryModel>.Create(finalQuery, model.PageNumber.Value, model.PageSize.Value);

            if(paginatedList.Items.Count() > 0)
            {
                for(int i = 0; i < paginatedList.Items.Count(); i++)
                {
                    // Approve status
                    if (paginatedList.Items[i].IsApprove == NewsApproveStatus.Active)
                    {
                        paginatedList.Items[i].IsApproveName = "Active";
                    }
                    else if (paginatedList.Items[i].IsApprove == NewsApproveStatus.New)
                    {
                        paginatedList.Items[i].IsApproveName = "New";
                    }
                    else if (paginatedList.Items[i].IsApprove == NewsApproveStatus.InActive)
                    {
                        paginatedList.Items[i].IsApproveName = "Inactive";
                    }
                    else if (paginatedList.Items[i].IsApprove == NewsApproveStatus.Lock)
                    {
                        paginatedList.Items[i].IsApproveName = "Lock";
                    }
                }
            }

            return paginatedList;
        }
    }
}
