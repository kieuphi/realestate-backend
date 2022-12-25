using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Application.Project.Queries;
using General.Domain.Entities;
using General.Domain.Entities.ProjectElementEntities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace General.Application.Project.Commands
{
    public class CreateProjectCommand : IRequest<Result>
    {
        public CreateProjectModel Model { set; get; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProjectEntity> _repository;
        private readonly ILogger<CreateProjectCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICommonFunctionService _commonFunctionService;

        public CreateProjectCommandHandler(
            IMapper mapper,
            IAsyncRepository<ProjectEntity> repository,
            ILogger<CreateProjectCommandHandler> logger,
            IApplicationDbContext context,
            IMediator mediator,
            ICommonFunctionService commonFunctionService
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var project = await _context.Project.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (project != null)
            {
                return Result.Failure($"The specified Project is invalid: {newId}");
            }

            if (model.ProjectSellers != null && model.ProjectSellers.Count() > 3)
            {
                return Result.Failure("Not allowed more than 3 sellers");
            }

            ProjectEntity entity = new ProjectEntity()
            {
                Id = newId,
                Slug = _commonFunctionService.GenerateFriendlyUrl(model.ProjectEn, 0),
                ProjectVi = model.ProjectVi,
                ProjectEn = model.ProjectEn,
                Descriptions = model.Descriptions,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CoverImage = model.CoverImage,
                ProjectLogo = model.ProjectLogo,
                Longtitude = model.Longtitude,
                Latitude = model.Latitude,
                Video = model.Video,
                VirtualTour = model.VirtualTour,
                FloorPlans = model.FloorPlans,
                MapViewImage = model.MapViewImage,
                Status = ProjectStatus.CommingSoon,
                ProvinceCode = model.ProvinceCode,
                DistrictCode = model.DistrictCode,
                WardCode = model.WardCode,
                Street = model.Street,
                Developer = model.Developer,
                PropertyTypeId = model.PropertyTypeId,
                IsApprove = ProjectApproveStatus.New,
            };

            CreateProjectElements(model, newId);

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetProjectByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }

        private void CreateProjectElements(CreateProjectModel model, Guid projectId)
        {
            // Project - Images
            if (model.ProjectImages != null && model.ProjectImages.Count() > 0)
            {
                foreach (var item in model.ProjectImages)
                {
                    _context.ProjectImage.Add(new ProjectImageEntity
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        Name = item.Name,
                        ImagesPath = item.ImagesPath
                    });
                };
            }

            // Project - Feature
            if (model.ProjectFeatures != null && model.ProjectFeatures.Count() > 0)
            {
                foreach (var item in model.ProjectFeatures)
                {
                    _context.ProjectFeature.Add(new ProjectFeatureEntity
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        ProjectFeatureId = item.ProjectFeatureId
                    });
                };
            }

            // Project - Seller
            if (model.ProjectSellers != null && model.ProjectSellers.Count() > 0)
            {
                foreach (var item in model.ProjectSellers)
                {
                    _context.ProjectSeller.Add(new ProjectSellerEntity
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        UserId = item.UserId
                    });
                };
            }
        }
    }
}
