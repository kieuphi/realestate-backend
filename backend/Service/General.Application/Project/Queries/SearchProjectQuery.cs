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
    public class SearchProjectQuery : IRequest<PaginatedList<ProjectModel>>
    {
        public SearchProjectModel Model { set; get; }
    }

    public class SearchProjectQueryHandler : IRequestHandler<SearchProjectQuery, PaginatedList<ProjectModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProjectEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IConvertVietNameseService _convertVietNameseService;
        private readonly IHandleProjectService _handleProjectService;

        public SearchProjectQueryHandler(
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

        public async Task<PaginatedList<ProjectModel>> Handle(SearchProjectQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var queryData = _context.Project.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == ProjectApproveStatus.Active).AsNoTracking();
            string host = _commonFunctionService.ConvertImageUrl("");

            if (!string.IsNullOrEmpty(model.ProjectName))
            {
                string projectName = model.ProjectName.ToLower().Trim();
                queryData = queryData.Where(x => x.ProjectEn.ToLower().Contains(projectName) || x.ProjectVi.ToLower().Contains(projectName));
            }

            if (!string.IsNullOrEmpty(model.Location))
            {
                queryData = queryData.Where(x => x.ProvinceCode == model.Location);
            }

            if (!string.IsNullOrEmpty(model.PropertyTypeId))
            {
                queryData = queryData.Where(x => x.PropertyTypeId == model.PropertyTypeId);
            }

            if (model.Status != null)
            {
                queryData = queryData.Where(x => x.Status == model.Status);
            }

            var projects = await queryData
                    .OrderByDescending(x => x.CreateTime)
                    .ProjectTo<ProjectModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            if (projects.Count() > 0)
            {
                var projectView = await _context.ProjectViewCount.ToListAsync();
                for(int i = 0; i < projects.Count(); i++)
                {
                    var viewCountProject = projectView.Where(x => x.ProjectId == projects[i].Id).FirstOrDefault();
                    projects[i].ViewCount = viewCountProject != null ? viewCountProject.ViewCount : 0;
                }
            }

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
