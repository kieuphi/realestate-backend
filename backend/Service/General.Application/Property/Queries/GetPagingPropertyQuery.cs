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
    public class GetPagingPropertyQuery : IRequest<PaginatedList<ListPropertyModel>>
    {
        public PagingPropertyModel PagingModel { set; get; }
    }

    public class GetPagingPropertyQueryHandler : IRequestHandler<GetPagingPropertyQuery, PaginatedList<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandlePropertyService _handlePropertyService;

        public GetPagingPropertyQueryHandler(
            IApplicationDbContext context,
            IAsyncRepository<PropertyEntity> repository,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IHandlePropertyService handlePropertyService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<PaginatedList<ListPropertyModel>> Handle(GetPagingPropertyQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;

            var properties = await _context.Property
                        .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active)
                        .AsNoTracking()
                        .OrderByDescending(x => x.ApproveDate)
                        .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

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
