using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.SocialNetwork.Queries
{
    public class GetSocialNetworkByIdQuery : IRequest<SocialNetworkModel>
    {
        public Guid Id { set; get; }
    }

    public class GetSocialNetworkByIdQueryHandler : IRequestHandler<GetSocialNetworkByIdQuery, SocialNetworkModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetSocialNetworkByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<SocialNetworkModel> Handle(GetSocialNetworkByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var result = await _context.SocialNetwork
                                       .Where(x => x.IsDeleted == DeletedStatus.False && x.Id == request.Id)
                                       .AsNoTracking()
                                       .ProjectTo<SocialNetworkModel>(_mapper.ConfigurationProvider)
                                       .FirstOrDefaultAsync();

            result.IConUrl = !string.IsNullOrEmpty(result.ICon) ? host + result.ICon : "";

            return result;
        }
    }
}
