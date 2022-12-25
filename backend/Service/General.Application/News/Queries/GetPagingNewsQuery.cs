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

namespace General.Application.News.Queries
{
    public class GetPagingNewsQuery : IRequest<PaginatedList<ListNewsModel>>
    {
        public PagingNewsModel PagingModel { set; get; }
    }

    public class GetPagingNewsQueryHandler : IRequestHandler<GetPagingNewsQuery, PaginatedList<ListNewsModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetPagingNewsQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<ListNewsModel>> Handle(GetPagingNewsQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            string host = _commonFunctionService.ConvertImageUrl("");
            var news = await _context.News
                .Where(x => x.IsApprove == NewsApproveStatus.Active &&
                    x.IsDeleted == DeletedStatus.False)
                .AsNoTracking()
                .OrderByDescending(x => x.ApproveDate)
                .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = news.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListNewsModel>.Create(news, model.PageNumber.Value, model.PageSize.Value);

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
