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

namespace General.Application.NewsCategory.Queries
{
    public class GetNewsCategoryByIdQuery : IRequest<NewsCategoryModel>
    {
        public Guid Id { set; get; }
    }

    public class GetNewsCategoryByIdQueryHandler : IRequestHandler<GetNewsCategoryByIdQuery, NewsCategoryModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetNewsCategoryByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<NewsCategoryModel> Handle(GetNewsCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.NewsCategory
                .Where(x => x.IsDeleted == DeletedStatus.False && x.Id == request.Id)
                .AsNoTracking()
                .ProjectTo<NewsCategoryModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
