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

namespace General.Application.News.Queries
{
    public class GetNextNewsQuery : IRequest<ListNewsModel>
    {
        public Guid CurrentNewsId { set; get; }
    }

    public class GetNextNewsQueryHandler : IRequestHandler<GetNextNewsQuery, ListNewsModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetNextNewsQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<ListNewsModel> Handle(GetNextNewsQuery request, CancellationToken cancellationToken)
        {
            var result = new ListNewsModel();
            string host = _commonFunctionService.ConvertImageUrl("");

            var currentNews = await _context.News
                .Where(x => x.Id == request.CurrentNewsId)
                .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            var newsByCategory = await _context.News
                .Where(x => x.CategoryId == currentNews.CategoryId &&
                    x.IsDeleted == DeletedStatus.False &&
                    x.IsApprove == NewsApproveStatus.Active)
                .OrderBy(x => x.ApproveDate)
                .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            int currentNewsIndex = newsByCategory.FindIndex(x => x.Id == request.CurrentNewsId);

            if (currentNewsIndex < newsByCategory.Count() - 1)
            {
                var nextNews = newsByCategory[currentNewsIndex + 1];
                nextNews.ImagePathUrl = !string.IsNullOrEmpty(nextNews.ImageUrl) ? host + nextNews.ImageUrl : "";

                result = nextNews;
            }

            return result;
        }
    }
}
