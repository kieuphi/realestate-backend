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

namespace General.Application.Property.Queries
{
    public class SearchPropertyForAdminQuery : IRequest<PaginatedList<ListPropertyModel>>
    {
        public SearchingPropertyForAdminModel SearchModel { set; get; }
    }

    public class SearchPropertyForAdminQueryHandler : IRequestHandler<SearchPropertyForAdminQuery, PaginatedList<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandlePropertyService _handlePropertyService;

        public SearchPropertyForAdminQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyEntity> repository,
            ICommonFunctionService commonFunctionService,
            IHandlePropertyService handlePropertyService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(repository));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<PaginatedList<ListPropertyModel>> Handle(SearchPropertyForAdminQuery request, CancellationToken cancellationToken)
        {
            var model = request.SearchModel;

            var queryData = _context.Property
                .Where(x => x.IsTemp == false)
                    .AsNoTracking();

            //string coordinatesProvince = "";

            if (model.IsApprove > 0 && model.IsApprove != null)
            {
                queryData = queryData.Where(p => p.IsApprove == model.IsApprove);
            }
            else if (model.IsApprove == PropertyApproveStatus.Lock)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.True && x.IsApprove == PropertyApproveStatus.Lock);
            }
            else if (model.IsApprove == 0 || model.IsApprove == null)
            {
                queryData = queryData.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove != PropertyApproveStatus.Lock);
            }

            if (!string.IsNullOrEmpty(model.PropertyNumber))
            {
                queryData = queryData.Where(x => x.PropertyNumber.ToLower().Contains(model.PropertyNumber.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(model.PropertyTypeId))
            {
                queryData = queryData.Where(p => p.PropertyTypeId == model.PropertyTypeId);
            }

            if (!string.IsNullOrEmpty(model.TransactionTypeId))
            {
                queryData = queryData.Where(p => p.TransactionTypeId == model.TransactionTypeId);
            }

            var finalQuery = queryData
                            .OrderByDescending(x => x.CreateTime)
                            .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);

            if(model.IsApprove != null && model.IsApprove == PropertyApproveStatus.Active)
            {
                finalQuery = finalQuery.OrderByDescending(x => x.ApproveDate);
            }

            if (model.SortingModel != null)
            {
                if (model.SortingModel.LowestPrice == true)
                {
                    finalQuery = queryData
                               .OrderBy(x => x.Price)
                               .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.HighestPrice == true)
                {
                    finalQuery = queryData
                               .OrderByDescending(x => x.Price)
                               .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.Oldest == true)
                {
                    finalQuery = queryData
                              .OrderBy(x => x.ApproveDate)
                              .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.Newest == true)
                {
                    finalQuery = queryData
                              .OrderByDescending(x => x.ApproveDate)
                              .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }
            }

            var properties = await finalQuery.ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = properties.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListPropertyModel>.Create(properties, model.PageNumber.Value, model.PageSize.Value);
            paginatedList.Items = await _handlePropertyService.JoinPropertyElements(paginatedList.Items);

            return paginatedList;
        }
    }
}
