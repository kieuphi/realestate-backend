using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;
using System.Linq;
using General.Domain.Entities.ProjectElementEntities;
using General.Domain.Enums;

namespace General.Application.Project.Commands
{
    public class UpdateProjectCommand : IRequest<Result>
    {
        public UpdateProjectModel Model { set; get; }
        public Guid ProjectId { set; get; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateProjectCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.Project.FindAsync(request.ProjectId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Project not exists." });
            }
            //if (entity.IsTemp == false)
            //{
            //    return Result.Failure("Cannot edit the available project");
            //}
            if (model.ProjectSellers != null && model.ProjectSellers.Count() > 3)
            {
                return Result.Failure("Not allowed more than 3 sellers");
            }

            entity.ProjectVi = model.ProjectVi;
            entity.ProjectEn = model.ProjectEn;
            entity.Descriptions = model.Descriptions;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.CoverImage = model.CoverImage;
            entity.ProjectLogo = model.ProjectLogo;
            entity.Longtitude = model.Longtitude;
            entity.Latitude = model.Latitude;
            entity.Video = model.Video;
            entity.VirtualTour = model.VirtualTour;
            entity.FloorPlans = model.FloorPlans;
            entity.MapViewImage = model.MapViewImage;
            entity.Status = model.Status;
            entity.ProvinceCode = model.ProvinceCode;
            entity.DistrictCode = model.DistrictCode;
            entity.WardCode = model.WardCode;
            entity.Street = model.Street;
            entity.Developer = model.Developer;
            entity.PropertyTypeId = model.PropertyTypeId;

            CreateProjectElements(model, request.ProjectId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private void CreateProjectElements(CreateProjectModel model, Guid projectId)
        {
            // Project - Image
            if (model.ProjectImages != null && model.ProjectImages.Count() > 0)
            {
                var projectImages = _context.ProjectImage.Where(x => x.ProjectId == projectId);
                _context.ProjectImage.RemoveRange(projectImages);

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
            else
            {
                var projectImages = _context.ProjectImage.Where(x => x.ProjectId == projectId);
                _context.ProjectImage.RemoveRange(projectImages);
            }

            // Project - Feature
            if (model.ProjectFeatures != null && model.ProjectFeatures.Count() > 0)
            {
                var projectViews = _context.ProjectFeature.Where(x => x.ProjectId == projectId);
                _context.ProjectFeature.RemoveRange(projectViews);

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
            else
            {
                var projectViews = _context.ProjectFeature.Where(x => x.ProjectId == projectId);
                _context.ProjectFeature.RemoveRange(projectViews);
            }

            // Project - Seller
            if (model.ProjectSellers != null && model.ProjectSellers.Count() > 0)
            {
                var projectSellers = _context.ProjectSeller.Where(x => x.ProjectId == projectId);
                _context.ProjectSeller.RemoveRange(projectSellers);

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
            else
            {
                var projectSellers = _context.ProjectSeller.Where(x => x.ProjectId == projectId);
                _context.ProjectSeller.RemoveRange(projectSellers);
            }
        }
    }
}
