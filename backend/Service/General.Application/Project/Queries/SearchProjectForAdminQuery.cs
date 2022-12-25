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
    public class SearchProjectForAdminQuery : IRequest<PaginatedList<ProjectModel>>
    {
        public SearchProjectForAdminModel Model { set; get; }
    }

    public class SearchProjectForAdminQueryHandler : IRequestHandler<SearchProjectForAdminQuery, PaginatedList<ProjectModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProjectEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IConvertVietNameseService _convertVietNameseService;
        private readonly IHandleProjectService _handleProjectService;

        public SearchProjectForAdminQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<ProjectEntity> repository,
            ICommonFunctionService commonFunctionService,
            IConvertVietNameseService convertVietNameseService,
            IHandleProjectService handleProjectService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(repository));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
            _handleProjectService = handleProjectService ?? throw new ArgumentNullException(nameof(handleProjectService));
        }

        public async Task<PaginatedList<ProjectModel>> Handle(SearchProjectForAdminQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var queryData = _context.Project.AsNoTracking();
            string host = _commonFunctionService.ConvertImageUrl("");

            // project name
            if (!string.IsNullOrEmpty(model.ProjectName))
            {
                string projectName = model.ProjectName.ToLower().Trim();
                queryData = queryData.Where(x => x.ProjectEn.ToLower().Contains(projectName) || x.ProjectVi.ToLower().Contains(projectName));
            }

            // property type
            if (!string.IsNullOrEmpty(model.PropertyTypeId))
            {
                queryData = queryData.Where(x => x.PropertyTypeId == model.PropertyTypeId);
            }

            // Apporve status
            if (model.IsApprove > 0 && model.IsApprove != null)
            {
                queryData = queryData.Where(p => p.IsApprove == model.IsApprove);
            }
            else if (model.IsApprove == ProjectApproveStatus.Lock)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.True && x.IsApprove == ProjectApproveStatus.Lock);
            }
            else if (model.IsApprove == 0 || model.IsApprove == null)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove != ProjectApproveStatus.Lock);
            }

            // status
            if (model.Status > 0)
            {
                queryData = queryData.Where(x => x.Status == model.Status);
            }

            var projects = await queryData
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ProjectModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if (model.SortingModel != null)
            {
                if (model.SortingModel.Latest == true)
                {
                    projects = projects.OrderByDescending(x => x.CreateTime).ToList();
                }

                if (model.SortingModel.Oldest == true)
                {
                    projects = projects.OrderBy(x => x.CreateTime).ToList();
                }

                if (model.SortingModel.MostView == true)
                {
                    projects = projects.OrderByDescending(x => x.ViewCount).ToList();
                }

                if (model.SortingModel.LeastView == true)
                {
                    projects = projects.OrderBy(x => x.ViewCount).ToList();
                }
            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = projects.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ProjectModel>.Create(projects, model.PageNumber.Value, model.PageSize.Value);
            paginatedList.Items = await _handleProjectService.JoinProjectElements(paginatedList.Items);

            return paginatedList;
        }
    }
}
