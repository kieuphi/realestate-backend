using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Banner.Query
{
    public class GetBannerByIdQuery : IRequest<BannerModel>
    {
        public Guid Id { set; get; }
    }

    public class GetBannerByIdQueryHandler : IRequestHandler<GetBannerByIdQuery, BannerModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetBannerByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<BannerModel> Handle(GetBannerByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");

            var result = await _context.Banner
                            .Where(x => x.IsDeleted == DeletedStatus.False && x.Id == request.Id)
                            .AsNoTracking()
                            .ProjectTo<BannerModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            result.ImagePathUrl = !string.IsNullOrEmpty(result.ImageUrl) ? host + result.ImageUrl : "";

            return result;
        }
    }
}
