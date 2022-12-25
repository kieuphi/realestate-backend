using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.SocialNetwork.Queries
{
    public class GetPagingSocialNetworkQuery : IRequest<PaginatedList<SocialNetworkModel>>
    {
        public PagingSocialNetworkModel PagingModel { set; get; }
    }

    public class GetPagingSocialNetworkQueryHandler : IRequestHandler<GetPagingSocialNetworkQuery, PaginatedList<SocialNetworkModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<SocialNetworkEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetPagingSocialNetworkQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<SocialNetworkEntity> repository,
            ICommonFunctionService commonFunctionService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<SocialNetworkModel>> Handle(GetPagingSocialNetworkQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            string host = _commonFunctionService.ConvertImageUrl("");
            var result = await _repository
                        .WhereIgnoreDelete(null)
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<SocialNetworkModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if (result.Count() > 0)
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    result[i].IConUrl = !string.IsNullOrEmpty(result[i].ICon) ? host + result[i].ICon : "";
                }
            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<SocialNetworkModel>(result, result.Count, 1, result.Count);
            }

            var paginatedList = PaginatedList<SocialNetworkModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
