using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using Common.Shared.Models;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Property.Queries
{
    public class GetNearestPropertyQuery : IRequest<PaginatedList<ListPropertyModel>>
    {
        public PagingNearestPropertyModel Model { set; get; }
    }

    public class GetNearestPropertyQueryHandler : IRequestHandler<GetNearestPropertyQuery, PaginatedList<ListPropertyModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandlePropertyService _handlePropertyService;

        public GetNearestPropertyQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService,
            IHandlePropertyService handlePropertyService
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<PaginatedList<ListPropertyModel>> Handle(GetNearestPropertyQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var result = new List<ListPropertyModel>();

            if(model.Ids != null && model.Ids.Count() > 0)
            {
                var entity = await _context.Property
                               .Where(x => x.IsDeleted == DeletedStatus.False 
                                    && x.IsApprove == PropertyApproveStatus.Active 
                                    && model.Ids.Contains(x.Id))
                               .AsNoTracking()
                               .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider).ToListAsync();

                List<ListPropertyModel> properties = new List<ListPropertyModel>();
                for (int i = 0; i < model.Ids.Count(); i++)
                {
                    var item = entity.Where(x => x.Id == model.Ids[i]).FirstOrDefault();
                    if (item != null)
                    {
                        properties.Add(item);
                    }
                }

                result = properties;
            } 
            else
            {
                var properties = new List<ListPropertyModel>();
                result = properties;
            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = result.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PaginatedList<ListPropertyModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);
            paginatedList.Items = await _handlePropertyService.JoinPropertyElements(paginatedList.Items);

            return paginatedList;
        }
    }
}
