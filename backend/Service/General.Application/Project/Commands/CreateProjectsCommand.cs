using AutoMapper;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Domain.Entities.ProjectElementEntities;
using Common.Shared.Enums;

namespace General.Application.Project.Commands
{
    public class CreateProjectsCommand : IRequest<Result>
    {
        public List<ImportProjectResultModel> Model { set; get; }
    }

    public class CreateProjectsCommandHandler : IRequestHandler<CreateProjectsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHandlePropertyService _handlePropertyService;
        private readonly ICommonFunctionService _commonFunctionService;

        public CreateProjectsCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyEntity> repository,
            ICurrentUserService currentUserService,
            IHandlePropertyService handlePropertyService,
            ICommonFunctionService commonFunctionService
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(CreateProjectsCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            int count = 0;

            foreach(var item in model)
            {
                count = count + 1;
                var newId = Guid.NewGuid();
                ProjectEntity entity = new ProjectEntity();

                entity.Id = newId;
                entity.Slug = _commonFunctionService.GenerateFriendlyUrl(item.ProjectEn, count);
                entity.ProjectVi = item.ProjectVi;
                entity.ProjectEn = item.ProjectEn;
                entity.Descriptions = item.Descriptions;
                if (!string.IsNullOrEmpty(item.StartDate))
                {
                    entity.StartDate = DateTime.Parse((item.StartDate + "-01-01 00:00"));
                }
                if (!string.IsNullOrEmpty(item.EndDate))
                {
                    entity.EndDate = DateTime.Parse((item.EndDate + "-01-01 00:00"));
                }
                //entity.CoverImage = item.CoverImage;
                //entity.ProjectLogo = item.ProjectLogo;
                entity.Longtitude = item.Longtitude;
                entity.Latitude = item.Latitude;
                entity.Video = item.Video;
                entity.VirtualTour = item.VirtualTour;
                entity.FloorPlans = item.FloorPlans;
                //entity.MapViewImage = item.MapViewImage;
                entity.Status = item.Status;
                entity.ProvinceCode = item.ProvinceCode;
                entity.DistrictCode = item.DistrictCode;
                entity.WardCode = item.WardCode;
                entity.Street = item.Street;
                entity.Developer = item.Developer;
                entity.PropertyTypeId = item.PropertyTypeId;
                entity.IsApprove = ProjectApproveStatus.New;
                entity.IsDeleted = DeletedStatus.False;

                _context.Project.Add(entity);

                CreateProjectElements(item, newId);
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }

        private void CreateProjectElements(ImportProjectResultModel model, Guid projectId)
        {
            // Project - Feature
            if (model.ProjectFeatureIds != null && model.ProjectFeatureIds.Count() > 0)
            {
                foreach (var item in model.ProjectFeatureIds)
                {
                    _context.ProjectFeature.Add(new ProjectFeatureEntity
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        ProjectFeatureId = item
                    });
                };
            }

            // Project - Images
            //if (model.ProjectImages != null && model.ProjectImages.Count() > 0)
            //{
            //    foreach (var item in model.ProjectImages)
            //    {
            //        _context.ProjectImage.Add(new ProjectImageEntity
            //        {
            //            Id = Guid.NewGuid(),
            //            ProjectId = projectId,
            //            Name = item.Name,
            //            ImagesPath = item.ImagesPath
            //        });
            //    };
            //}
        }
    }
}
