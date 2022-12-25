using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using System.Linq;
using AutoMapper;
using General.Domain.Entities;
using General.Domain.Entities.PropertyElementEntities;
using General.Domain.Models.PropertyElementModels;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Property.Commands
{
    public class UpdatePropertyByUserCommand : IRequest<Result>
    {
        public CreatePropertyModel Model { set; get; }
        public Guid PropertyId { set; get; }
    }

    public class UpdatePropertyByUserCommandHandler : IRequestHandler<UpdatePropertyByUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public UpdatePropertyByUserCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService userService
        ) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }


        public async Task<Result> Handle(UpdatePropertyByUserCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }
            if (entity.IsTemp == false)
            {
                return Result.Failure("Cannot edit the available property");
            }
            if (entity.CreateBy != _userService.UserName)
            {
                return Result.Failure("Not privilege");
            }

            if (model.TransactionTypeId != entity.TransactionTypeId)
            {
                return Result.Failure("Cannot change Transaction Type");
            }

            if (model.PropertyImages.Count() < 4)
            {
                return Result.Failure("Minimum 4 images.");
            }

            if (entity.IsTemp == true && model.IsSubmit == true)
            {
                entity.IsTemp = false;
                entity.IsApprove = Domain.Enums.PropertyApproveStatus.New;
            }

            // summary
            entity.PropertyTypeId = model.PropertyTypeId;
            //entity.TransactionTypeId = model.TransactionTypeId;
            entity.CoverImage = model.CoverImage;
            entity.VideoLink = model.VideoLink;
            entity.VirtualVideoLink = model.VirtualVideoLink;

            // address
            entity.Address = model.Address;
            entity.ProvinceCode = model.ProvinceCode;
            entity.DistrictCode = model.DistrictCode;
            entity.WardCode = model.WardCode;
            entity.Street = model.Street;
            entity.ProjectId = model.ProjectId;
            entity.ProjectName = model.ProjectName;
            entity.PropertyAddressVi = model.PropertyAddressVi;
            entity.PropertyAddressEn = model.PropertyAddressEn;

            // description
            entity.Title = model.Title;
            entity.Descriptions = model.Descriptions;

            // informations
            entity.LotSize = model.LotSize != null ? model.LotSize : 0;
            entity.Price = model.Price != null ? model.Price : 0;
            entity.USDPrice = model.USDPrice != null ? model.USDPrice : 0;
            entity.CurrencyId = model.CurrencyId;
            entity.BedroomId = model.BedroomId;
            entity.BathroomId = model.BathroomId;
            entity.FloorsNumber = model.FloorsNumber != null ? model.FloorsNumber : 0;
            entity.TotalBuildingFloors = model.TotalBuildingFloors != null ? model.TotalBuildingFloors : 0;
            entity.YearCompleted = model.YearCompleted;
            entity.Longitude = model.Longitude;
            entity.Latitude = model.Latitude;

            await CreatePropertyElements(model, entity.Id);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private async Task CreatePropertyElements(CreatePropertyModel model, Guid propertyId)
        {
            // Property - Images
            if (model.PropertyImages != null)
            {
                var listDelete = await _context.PropertyImage.Where(x => x.PropertyId == propertyId).ToListAsync();
                _context.PropertyImage.RemoveRange(listDelete);

                foreach (var item in model.PropertyImages)
                {
                    _context.PropertyImage.Add(new PropertyImageEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        Name = item.Name,
                        Notes = item.Notes,
                        ImagesPath = item.ImagesPath
                    });
                };
            }

            // Property - View
            if (model.PropertyViews != null)
            {
                var listDelete = await _context.PropertyView.Where(x => x.PropertyId == propertyId).ToListAsync();
                _context.PropertyView.RemoveRange(listDelete);
                foreach (var item in model.PropertyViews)
                {
                    _context.PropertyView.Add(new PropertyViewEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        ViewId = item.ViewId
                    });
                };
            }

            // Property - Amenities Nearby 
            if (model.PropertyAmenitiesNearbys != null)
            {
                var listDelete = await _context.PropertyAmenitiesNearby.Where(x => x.PropertyId == propertyId).ToListAsync();
                _context.PropertyAmenitiesNearby.RemoveRange(listDelete);
                foreach (var item in model.PropertyAmenitiesNearbys)
                {
                    _context.PropertyAmenitiesNearby.Add(new PropertyAmenitiesNearbyEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        AmenitiesNearbyId = item.AmenitiesNearbyId
                    });
                };
            }
        }
    }
}
