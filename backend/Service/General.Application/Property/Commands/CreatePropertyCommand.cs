using MediatR;
using System;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Domain.Entities;
using General.Application.Interfaces;
using General.Application.Common.Interfaces;
using General.Domain.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using General.Application.Property.Queries;
using Newtonsoft.Json.Linq;
using System.IO;
using General.Domain.Entities.PropertyElementEntities;
using General.Domain.Enums;

namespace General.Application.Property.Commands
{
    public class CreatePropertyCommand : IRequest<Result>
    {
        public CreatePropertyModel Model { set; get; }
        public bool IsSaveTemp { get; set; }
    }

    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result>
    {
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IHandlePropertyService _handlePropertyService;
        private readonly ICommonFunctionService _commonFunctionService;

        public CreatePropertyCommandHandler(
            IAsyncRepository<PropertyEntity> repository,
            IApplicationDbContext context,
            IMediator mediator,
            IHandlePropertyService handlePropertyService,
            ICommonFunctionService commonFunctionService
        ) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var requestModel = request.Model;

            var newId = Guid.NewGuid();
            string masterData = File.ReadAllText("MasterData.json");

            string propertyNumber = "";
            var transactionType = JObject.Parse(masterData)["transactionType"]
                          .Where(n => n["id"].Value<string>() == requestModel.TransactionTypeId)
                          .Select(n => new { Value = n["notation"] }).FirstOrDefault();
            if(transactionType != null)
            {
                propertyNumber = await _handlePropertyService.GeneratePropertyNumber(transactionType.Value.ToString());
            }

            var Property = await _context.Property.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (Property != null)
            {
                return Result.Failure($"The specified Property is invalid: {newId}");
            }

            if (requestModel.PropertyImages.Count() < 4)
            {
                return Result.Failure("Minimum 4 images.");
            }

            // catch required
            string[] requiredFields = { "TransactionTypeId", "PropertyTypeId", "ProvinceCode", "DistrictCode", "PropertyAddressVi",
                    "Title", "Descriptions", "LotSize", "Price", "CurrencyId", "Longitude", "Latitude"};
            string requiredMessage = "";
            foreach (var field in requiredFields)
            {
                if (requestModel.GetType().GetProperty(field).GetValue(requestModel, null) == null)
                {
                    requiredMessage = requiredMessage + field + ", ";
                }
            }

            if (requestModel.PropertyViews == null || requestModel.PropertyViews.Count() == 0)
            {
                requiredMessage = requiredMessage + "View";
            }

            if (!string.IsNullOrEmpty(requiredMessage))
            {
                requiredMessage = requiredMessage + "are required!";
                return Result.Failure(requiredMessage);
            }

            // entity
            PropertyEntity entity = new PropertyEntity()
            {
                Id = newId,
                Slug = _commonFunctionService.GenerateFriendlyUrl(requestModel.Title, 0),
                PropertyNumber = propertyNumber,

                // summary
                PropertyTypeId = requestModel.PropertyTypeId,
                TransactionTypeId = requestModel.TransactionTypeId,
                CoverImage = requestModel.CoverImage,
                VideoLink = requestModel.VideoLink,
                VirtualVideoLink = requestModel.VirtualVideoLink,

                // address
                Address = requestModel.Address,
                ProvinceCode = requestModel.ProvinceCode,
                DistrictCode = requestModel.DistrictCode,
                WardCode = requestModel.WardCode,
                Street = requestModel.Street,
                ProjectId = requestModel.ProjectId,
                ProjectName = requestModel.ProjectName,
                PropertyAddressVi = requestModel.PropertyAddressVi,
                PropertyAddressEn = requestModel.PropertyAddressEn,

                // description
                Title = requestModel.Title,
                Descriptions = requestModel.Descriptions,

                // informations
                LotSize = requestModel.LotSize != null ? requestModel.LotSize : 0,
                Price = requestModel.Price != null ? requestModel.Price : 0,
                USDPrice = requestModel.USDPrice != null ? requestModel.USDPrice : 0,
                CurrencyId = requestModel.CurrencyId,
                BedroomId = requestModel.BedroomId,
                BathroomId = requestModel.BathroomId,
                FloorsNumber = requestModel.FloorsNumber != null ? requestModel.FloorsNumber : 0,
                TotalBuildingFloors = requestModel.TotalBuildingFloors != null ? requestModel.TotalBuildingFloors : 0,
                YearCompleted = requestModel.YearCompleted,
                Longitude = requestModel.Longitude,
                Latitude = requestModel.Latitude,

                // status
                IsApprove = PropertyApproveStatus.New,

                // supplier
                SupplierId = requestModel.SupplierId,
                IsShowSupplier = false,

                // save temp
                IsTemp = request.IsSaveTemp
            };

            CreatePropertyElements(requestModel, newId);

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetPropertyByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }

        private void CreatePropertyElements(CreatePropertyModel model, Guid propertyId)
        {
            // Property - Images
            if (model.PropertyImages != null && model.PropertyImages.Count() > 0)
            {
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
            if(model.PropertyViews != null && model.PropertyViews.Count() > 0)
            {
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
            if (model.PropertyAmenitiesNearbys != null && model.PropertyAmenitiesNearbys.Count() > 0)
            {
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
