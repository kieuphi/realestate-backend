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

namespace General.Application.Property.Commands
{
    public class UpdatePropertyCommand : IRequest<Result>
    {
        public UpdatePropertyModel Model { set; get; }
        public Guid PropertyId { set; get; }
    }

    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdatePropertyCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }

            if (model.TransactionTypeId != entity.TransactionTypeId)
            {
                return Result.Failure("Cannot change Transaction Type");
            }

            if (model.IsShowSupplier == true && model.PropertySellers != null && model.PropertySellers.Count() > 2)
            {
                return Result.Failure("Not allowed more than 3 sellers when show supplier on website");
            }

            if (model.PropertyImages.Count() < 4)
            {
                return Result.Failure("Minimum 4 images.");
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
            entity.IsShowSupplier = model.IsShowSupplier;

            CreatePropertyElements(model, request.PropertyId);
            CreatePropertyMeetingNote(model.PropertyMeetingNote, request.PropertyId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private void CreatePropertyElements(UpdatePropertyModel model, Guid propertyId)
        {
            // Property - Image
            if (model.PropertyImages != null && model.PropertyImages.Count() > 0)
            {
                var propertyImages = _context.PropertyImage.Where(x => x.PropertyId == propertyId);
                _context.PropertyImage.RemoveRange(propertyImages);

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
            } else
            {
                var propertyImages = _context.PropertyImage.Where(x => x.PropertyId == propertyId);
                _context.PropertyImage.RemoveRange(propertyImages);
            }

            // Property - View
            if (model.PropertyViews != null && model.PropertyViews.Count() > 0)
            {
                var propertyViews = _context.PropertyView.Where(x => x.PropertyId == propertyId);
                _context.PropertyView.RemoveRange(propertyViews);

                foreach (var item in model.PropertyViews)
                {
                    _context.PropertyView.Add(new PropertyViewEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        ViewId = item.ViewId
                    });
                };
            } else
            {
                var propertyViews = _context.PropertyView.Where(x => x.PropertyId == propertyId);
                _context.PropertyView.RemoveRange(propertyViews);
            }

            // Property - Amenities Nearby 
            if (model.PropertyAmenitiesNearbys != null && model.PropertyAmenitiesNearbys.Count() > 0)
            {
                var propertyAmenitiesNearbys = _context.PropertyAmenitiesNearby.Where(x => x.PropertyId == propertyId);
                _context.PropertyAmenitiesNearby.RemoveRange(propertyAmenitiesNearbys);

                foreach (var item in model.PropertyAmenitiesNearbys)
                {
                    _context.PropertyAmenitiesNearby.Add(new PropertyAmenitiesNearbyEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        AmenitiesNearbyId = item.AmenitiesNearbyId
                    });
                };
            } else
            {
                var propertyAmenitiesNearbys = _context.PropertyAmenitiesNearby.Where(x => x.PropertyId == propertyId);
                _context.PropertyAmenitiesNearby.RemoveRange(propertyAmenitiesNearbys);
            }            
            
            // Property - Seller
            if (model.PropertySellers != null && model.PropertySellers.Count() > 0)
            {
                var propertySellers = _context.PropertySeller.Where(x => x.PropertyId == propertyId);
                _context.PropertySeller.RemoveRange(propertySellers);

                foreach (var item in model.PropertySellers)
                {
                    _context.PropertySeller.Add(new PropertySellerEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        UserId = item.UserId
                    });
                };
            } else
            {
                var propertySellers = _context.PropertySeller.Where(x => x.PropertyId == propertyId);
                _context.PropertySeller.RemoveRange(propertySellers);
            }
        }

        private void CreatePropertyMeetingNote(CreatePropertyMeetingNoteModel model, Guid propertyId)
        {
            if (model != null)
            {
                var meetingNote = _context.PropertyMeetingNote.Where(x => x.PropertyId == propertyId).FirstOrDefault();

                if (meetingNote == null)
                {
                    _context.PropertyMeetingNote.Add(new PropertyMeetingNoteEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        MeetingNoteTitle = model.MeetingNoteTitle,
                        MeetingNoteContent = model.MeetingNoteContent
                    });
                } else
                {
                    meetingNote.MeetingNoteTitle = model.MeetingNoteTitle;
                    meetingNote.MeetingNoteContent = model.MeetingNoteContent;
                }
            }
        }
    }
}
