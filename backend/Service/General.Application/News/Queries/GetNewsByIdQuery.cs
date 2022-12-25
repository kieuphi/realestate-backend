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

namespace General.Application.News.Queries
{
    public class GetNewsByIdQuery : IRequest<NewsModel>
    {
        public Guid Id { set; get; }
    }

    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetNewsByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<NewsModel> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");

            var result = await _context.News
                            .Where(x => x.IsDeleted == DeletedStatus.False && x.Id == request.Id)
                            .AsNoTracking()
                            .ProjectTo<NewsModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            var viewCount = await _context.NewsViewCount.Where(x => x.NewsId == result.Id).FirstOrDefaultAsync();
            if (viewCount != null)
            {
                result.ViewCount = viewCount.ViewCount != null ? Int32.Parse(viewCount.ViewCount.ToString()) : 0;
            }

            result.ImagePathUrl = !string.IsNullOrEmpty(result.ImageUrl) ? host + result.ImageUrl : "";

            return result;
        }
    }
}
