using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Project.Queries
{
    public class GetPagingProjectQuery : IRequest<PaginatedList<ProjectModel>>
    {
        public PagingProjectModel PagingModel { set; get; }
    }

    public class GetPagingProjectQueryHandler : IRequestHandler<GetPagingProjectQuery, PaginatedList<ProjectModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<ProjectEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandleProjectService _handleProjectService;

        public GetPagingProjectQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<ProjectEntity> repository,
            ICommonFunctionService commonFunctionService,
            IHandleProjectService handleProjectService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _handleProjectService = handleProjectService ?? throw new ArgumentNullException(nameof(handleProjectService));
        }

        public async Task<PaginatedList<ProjectModel>> Handle(GetPagingProjectQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var model = request.PagingModel;
            var result = await _context.Project
                        .Where(x => x.IsApprove == ProjectApproveStatus.Active && x.IsDeleted == DeletedStatus.False)
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ProjectModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = result.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ProjectModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);
            paginatedList.Items = await _handleProjectService.JoinProjectElements(paginatedList.Items);

            return paginatedList;
        }
    }
}
