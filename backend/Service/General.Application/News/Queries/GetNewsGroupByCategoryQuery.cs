using AutoMapper;
using Common.Shared.Models;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using General.Domain.Enums;
using Common.Shared.Enums;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace General.Application.News.Queries
{
    public class GetNewsGroupByCategoryQuery : IRequest<List<ListNewsGroupByCategoryModel>>
    {
    }

    public class GetNewsGroupByCategoryQueryHandler : IRequestHandler<GetNewsGroupByCategoryQuery, List<ListNewsGroupByCategoryModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetNewsGroupByCategoryQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<List<ListNewsGroupByCategoryModel>> Handle(GetNewsGroupByCategoryQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            List<ListNewsGroupByCategoryModel> result = new List<ListNewsGroupByCategoryModel>();

            var newsCategories = await _context.NewsCategory
                .Where(x => x.IsDeleted == DeletedStatus.False && 
                    x.IsApprove == NewsApproveStatus.Active)
                .OrderByDescending(x => x.ApproveDate)
                .ProjectTo<ListNewsGroupByCategoryModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (newsCategories.Count() > 0)
            {
                for(int i = 0; i < newsCategories.Count(); i++)
                {
                    newsCategories[i].News = newsCategories[i].News
                        .Where(x => x.IsApprove == NewsApproveStatus.Active && x.IsDeleted == DeletedStatus.False)
                        .OrderByDescending(x => x.ApproveDate).Take(3).ToList();

                    if (newsCategories[i].News.Count() > 0)
                    {
                        for (int j = 0; j < newsCategories[i].News.Count(); j++)
                        {
                            newsCategories[i].News[j].ImagePathUrl = !string.IsNullOrEmpty(newsCategories[i].News[j].ImageUrl) ? host + newsCategories[i].News[j].ImageUrl : "";
                        }

                        result.Add(newsCategories[i]);
                    }
                }
            }

            return result;
        }
    }
}
