using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
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
    public class GetAllPropertyQuery : IRequest<List<ListPropertyModel>>
    {
    }

    public class GetAllPropertyQueryHandler : IRequestHandler<GetAllPropertyQuery, List<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IHandlePropertyService _handlePropertyService;

        public GetAllPropertyQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IHandlePropertyService handlePropertyService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<List<ListPropertyModel>> Handle(GetAllPropertyQuery request, CancellationToken cancellationToken)
        {
            var properties = await _context.Property
                    .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active)
                    .AsNoTracking()
                    .OrderByDescending(x => x.ApproveDate)
                    .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            properties = await _handlePropertyService.JoinPropertyElements(properties);

            return properties;
        }
    }
}
