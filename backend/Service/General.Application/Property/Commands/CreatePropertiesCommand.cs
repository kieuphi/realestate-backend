using AutoMapper;
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
using Common.Shared.Models;
using Common.Shared.Enums;
using General.Domain.Entities.PropertyElementEntities;

namespace General.Application.Property.Commands
{
    public class CreatePropertiesCommand : IRequest<Result>
    {
        public List<ImportPropertyResultModel> Model { set; get; }
    }

    public class CreatePropertiesCommandHandler : IRequestHandler<CreatePropertiesCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHandlePropertyService _handlePropertyService;
        private readonly ICommonFunctionService _commonFunctionService;

        public CreatePropertiesCommandHandler(
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

        public async Task<Result> Handle(CreatePropertiesCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            int count = 0;

            foreach(var item in model)
            {
                var newId = Guid.NewGuid();
                count = count + 1;

                string masterData = File.ReadAllText("MasterData.json");

                string propertyNumber = "";
                var transactionType = JObject.Parse(masterData)["transactionType"]
                              .Where(n => n["id"].Value<string>() == item.TransactionTypeId)
                              .Select(n => new { Value = n["notation"] }).FirstOrDefault();
                if (transactionType != null)
                {
                    propertyNumber = await _handlePropertyService.GeneratePropertyNumber(transactionType.Value.ToString());
                }

                PropertyEntity entity = new PropertyEntity();
                entity.Id = newId;
                entity.PropertyNumber = propertyNumber;
                entity.Slug = _commonFunctionService.GenerateFriendlyUrl(item.Title, count);

                // summary
                entity.PropertyTypeId = item.PropertyTypeId;
                entity.TransactionTypeId = item.TransactionTypeId;
                //entity.CoverImage = item.CoverImage;
                entity.VideoLink = item.VideoLink;
                entity.VirtualVideoLink = item.VirtualVideoLink;

                // address
                // entity.Address = item.Address,
                entity.ProvinceCode = item.ProvinceCode;
                entity.DistrictCode = item.DistrictCode;
                entity.WardCode = item.WardCode;
                entity.Street = item.Street;
                // entity.ProjectId = item.ProjectId;
                // entity.ProjectName = item.ProjectName;
                entity.PropertyAddressVi = item.PropertyAddressVi;
                entity.PropertyAddressEn = item.PropertyAddressEn;

                // description
                entity.Title = item.Title;
                entity.Descriptions = item.Descriptions;

                // informations
                entity.LotSize = !string.IsNullOrEmpty(item.LotSize) ? decimal.Parse(item.LotSize.Replace(".", ",")) : 0;
                entity.Price = !string.IsNullOrEmpty(item.Price) ? decimal.Parse(item.Price.Replace(".", ",")) : 0;
                entity.USDPrice = !string.IsNullOrEmpty(item.USDPrice) ? decimal.Parse(item.USDPrice.Replace(".", ",")) : 0;
                entity.CurrencyId = item.CurrencyId;
                entity.BedroomId = item.BedroomId;
                entity.BathroomId = item.BathroomId;
                entity.FloorsNumber = !string.IsNullOrEmpty(item.FloorsNumber) ? Int32.Parse(item.FloorsNumber) : 0;
                entity.TotalBuildingFloors = !string.IsNullOrEmpty(item.TotalBuildingFloors) ? Int32.Parse(item.TotalBuildingFloors) : 0;
                if (!string.IsNullOrEmpty(item.YearCompleted))
                {
                    entity.YearCompleted = DateTime.Parse((item.YearCompleted + "-01-01 00:00"));
                }
                entity.Longitude = item.Longitude;
                entity.Latitude = item.Latitude;

                // status
                entity.IsApprove = PropertyApproveStatus.New;

                // supplier
                entity.SupplierId = Guid.Parse(_currentUserService.UserId);
                entity.IsShowSupplier = false;

                // status
                entity.IsTemp = false;
                entity.IsDeleted = DeletedStatus.False;

                _context.Property.Add(entity);

                CreatePropertyElements(item, newId);
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }

        private void CreatePropertyElements(ImportPropertyResultModel model, Guid propertyId)
        {
            // Property - View
            if (model.PropertyViewIds != null && model.PropertyViewIds.Count() > 0)
            {
                foreach (var item in model.PropertyViewIds)
                {
                    _context.PropertyView.Add(new PropertyViewEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        ViewId = item
                    });
                };
            }

            // Property - Amenities Nearby 
            if (model.PropertyAmenitiesNearbyIds != null && model.PropertyAmenitiesNearbyIds.Count() > 0)
            {
                foreach (var item in model.PropertyAmenitiesNearbyIds)
                {
                    _context.PropertyAmenitiesNearby.Add(new PropertyAmenitiesNearbyEntity
                    {
                        Id = Guid.NewGuid(),
                        PropertyId = propertyId,
                        AmenitiesNearbyId = item
                    });
                };
            }

            // Property - Images
            //if (model.PropertyImages != null && model.PropertyImages.Count() > 0)
            //{
            //    foreach (var item in model.PropertyImages)
            //    {
            //        _context.PropertyImage.Add(new PropertyImageEntity
            //        {
            //            Id = Guid.NewGuid(),
            //            PropertyId = propertyId,
            //            Name = item.Name,
            //            Notes = item.Notes,
            //            ImagesPath = item.ImagesPath
            //        });
            //    };
            //}
        }
    }
}
