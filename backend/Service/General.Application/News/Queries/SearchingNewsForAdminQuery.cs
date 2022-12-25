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

namespace General.Application.News.Queries
{
    public class SearchingNewsForAdminQuery : IRequest<PaginatedList<ListNewsModel>>
    {
        public SearchingNewsForAdminModel Model { set; get; }
    }

    public class SearchingNewsForAdminQueryHandler : IRequestHandler<SearchingNewsForAdminQuery, PaginatedList<ListNewsModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public SearchingNewsForAdminQueryHandler(IMapper mapper, IApplicationDbContext context, ICommonFunctionService commonFunctionService) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<ListNewsModel>> Handle(SearchingNewsForAdminQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var queryData = _context.News.AsNoTracking();
            string host = _commonFunctionService.ConvertImageUrl("");

            if (!string.IsNullOrEmpty(model.Title))
            {
                var title = model.Title.ToLower().Trim();
                queryData = queryData.Where(x => x.TitleVi.ToLower().Contains(title) || x.TitleEn.ToLower().Contains(title));
            }

            if (model.IsHotNew != null)
            {
                queryData = queryData.Where(x => x.IsHotNews == model.IsHotNew);
            }

            if (model.CategoryId != null && model.CategoryId != Guid.Empty)
            {
                queryData = queryData.Where(x => x.CategoryId == model.CategoryId);
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
                //queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove != NewsApproveStatus.Lock);
            }

            var finalQuery = await queryData
                    .OrderByDescending(x => x.CreateTime)
                    .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = finalQuery.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListNewsModel>.Create(finalQuery, model.PageNumber.Value, model.PageSize.Value);

            if(paginatedList.Items.Count() > 0)
            {
                for(int i = 0; i < paginatedList.Items.Count(); i++)
                {
                    paginatedList.Items[i].ImagePathUrl = !string.IsNullOrEmpty(paginatedList.Items[i].ImageUrl) ? host + paginatedList.Items[i].ImageUrl : "";

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
                        paginatedList.Items[i].IsApproveName = "InActive";
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
