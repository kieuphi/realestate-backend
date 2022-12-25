using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using General.Domain.Models.PropertyElementModels;
using General.Domain.Enums;
using General.Application.Common.Results;
using System.Collections.Generic;

namespace General.Application.Property.Queries
{
    public class GetByListIdQuery : IRequest<List<ListPropertyModel>>
    {
        public List<Guid> ListId { set; get; }
    }

    public class GetByListIdQueryHandler : IRequestHandler<GetByListIdQuery, List<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandlePropertyService _handlePropertyService;

        public GetByListIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IHandlePropertyService handlePropertyService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<List<ListPropertyModel>> Handle(GetByListIdQuery request, CancellationToken cancellationToken)
        {
            var properties = await _context.Property
                    .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active && request.ListId.Contains(x.Id))
                    .AsNoTracking()
                    .OrderByDescending(x => x.ApproveDate)
                    .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            properties = await _handlePropertyService.JoinPropertyElements(properties);

            return properties;
        }
    }
}
