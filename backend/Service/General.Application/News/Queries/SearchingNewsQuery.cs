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
    public class SearchingNewsQuery : IRequest<PaginatedList<ListNewsModel>>
    {
        public SearchingNewsModel Model { set; get; }
    }

    public class SearchingNewsQueryHandler : IRequestHandler<SearchingNewsQuery, PaginatedList<ListNewsModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public SearchingNewsQueryHandler(IMapper mapper, IApplicationDbContext context, ICommonFunctionService commonFunctionService) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<ListNewsModel>> Handle(SearchingNewsQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var queryData = _context.News
                .Where(x => x.IsDeleted == DeletedStatus.False && 
                    x.IsApprove == NewsApproveStatus.Active)
                .AsNoTracking();
            string host = _commonFunctionService.ConvertImageUrl("");

            if (!string.IsNullOrEmpty(model.Title))
            {
                var title = model.Title.ToLower().Trim();
                queryData = queryData.Where(x => x.TitleVi.ToLower().Contains(title) || x.TitleEn.ToLower().Contains(title));
            }

            var finalQuery = await queryData
                    .OrderByDescending(x => x.ApproveDate)
                    .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = finalQuery.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListNewsModel>.Create(finalQuery, model.PageNumber.Value, model.PageSize.Value);

            if (paginatedList.Items.Count() > 0)
            {
                var listViewCount = await _context.NewsViewCount.ToListAsync();
                for (int i = 0; i < paginatedList.Items.Count(); i++)
                {
                    var viewCount = listViewCount.Where(x => x.NewsId == paginatedList.Items[i].Id).FirstOrDefault();
                    paginatedList.Items[i].ViewCount = viewCount != null ? viewCount.ViewCount : 0;

                    paginatedList.Items[i].ImagePathUrl = !string.IsNullOrEmpty(paginatedList.Items[i].ImageUrl) ? host + paginatedList.Items[i].ImageUrl : "";
                }
            }

            return paginatedList;
        }
    }
}
