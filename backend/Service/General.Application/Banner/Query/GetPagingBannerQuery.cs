using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using General.Domain.Enums;

namespace General.Application.Banner.Query
{
    public class GetPagingBannerQuery : IRequest<PaginatedList<BannerModel>>
    {
        public PagingBannerModel Model { set; get; }
    }

    public class GetPagingBannerQueryHandler : IRequestHandler<GetPagingBannerQuery, PaginatedList<BannerModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<BannerEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetPagingBannerQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<BannerEntity> repository,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<BannerModel>> Handle(GetPagingBannerQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            string host = _commonFunctionService.ConvertImageUrl("");
            var banners = await _repository
                        .WhereIgnoreDelete(null)
                        .AsNoTracking()
                        .OrderBy(x => x.BannerOrder)
                        .ProjectTo<BannerModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if (banners.Count() > 0)
            {
                for (int i = 0; i < banners.Count(); i++)
                {
                    banners[i].ImagePathUrl = !string.IsNullOrEmpty(banners[i].ImageUrl) ? host + banners[i].ImageUrl : "";

                    if (banners[i].BannerType == BannerTypes.HomePage)
                    {
                        banners[i].BannerTypeName = "Home Page";
                    }

                    if (banners[i].BannerType == BannerTypes.ProjectPage)
                    {
                        banners[i].BannerTypeName = "Project Page";
                    }
                }
            }


            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<BannerModel>(banners, banners.Count, 1, banners.Count);
            }

            var paginatedList = PaginatedList<BannerModel>.Create(banners, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
