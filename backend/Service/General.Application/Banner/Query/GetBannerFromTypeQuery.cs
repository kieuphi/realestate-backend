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

namespace General.Application.Banner.Query
{
    public class GetBannerFromTypeQuery : IRequest<List<BannerModel>>
    {
        public BannerTypes Type { set; get; }
    }

    public class GetBannerFromTypeQueryHandler : IRequestHandler<GetBannerFromTypeQuery, List<BannerModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetBannerFromTypeQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<List<BannerModel>> Handle(GetBannerFromTypeQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");

            var result = await _context.Banner
                            .Where(x => x.IsDeleted == DeletedStatus.False && x.BannerType == request.Type)
                            .AsNoTracking()
                            .OrderBy(x => x.BannerOrder)
                            .ProjectTo<BannerModel>(_mapper.ConfigurationProvider).ToListAsync();

            if (result.Count() > 0)
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    result[i].ImagePathUrl = !string.IsNullOrEmpty(result[i].ImageUrl) ? host + result[i].ImageUrl : "";

                    if (result[i].BannerType == BannerTypes.HomePage)
                    {
                        result[i].BannerTypeName = "Home Page";
                    }

                    if (result[i].BannerType == BannerTypes.ProjectPage)
                    {
                        result[i].BannerTypeName = "Project Page";
                    }
                }
            }

            return result;
        }
    }
}
