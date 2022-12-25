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
    public class GetAllNewsQuery : IRequest<List<ListNewsModel>>
    {
    }

    public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, List<ListNewsModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetAllNewsQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<List<ListNewsModel>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");

            var result = await _context.News
                    .Where(x => x.IsDeleted == DeletedStatus.False)
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreateTime)
                    .ProjectTo<ListNewsModel>(_mapper.ConfigurationProvider).ToListAsync();

            if(result.Count() > 0)
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    result[i].ImagePathUrl = !string.IsNullOrEmpty(result[i].ImageUrl) ? host + result[i].ImageUrl : "";
                }
            }

            return result;
        }
    }
}
