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

namespace General.Application.TimeForPost.Queries
{
    public class GetPagingTimeForPostQuery : IRequest<PaginatedList<TimeForPostModel>>
    {
        public PagingTimeForPostModel PagingModel { set; get; }
    }

    public class GetPagingTimeForPostQueryHandler : IRequestHandler<GetPagingTimeForPostQuery, PaginatedList<TimeForPostModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<TimeForPostEntity> _repository;

        public GetPagingTimeForPostQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<TimeForPostEntity> repository
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PaginatedList<TimeForPostModel>> Handle(GetPagingTimeForPostQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            var project = await _repository
                        .WhereIgnoreDelete(null)
                        .AsNoTracking()
                        .OrderBy(x => x.Value)
                        .ProjectTo<TimeForPostModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<TimeForPostModel>(project, project.Count, 1, project.Count);
            }

            var paginatedList = PaginatedList<TimeForPostModel>.Create(project, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
