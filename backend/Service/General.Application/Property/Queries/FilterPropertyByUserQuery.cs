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
    public class FilterPropertyByUserQuery : IRequest<List<ListPropertyModel>>
    {
        public FilterPropertyByUserModel FilterData { get; set; }
    }

    public class FilterPropertyByUserQueryHandler : IRequestHandler<FilterPropertyByUserQuery, List<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHandlePropertyService _handlePropertyService;
        private readonly ICurrentUserService _userService;

        public FilterPropertyByUserQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IHandlePropertyService handlePropertyService,
            ICurrentUserService userService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<List<ListPropertyModel>> Handle(FilterPropertyByUserQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Property
                    .Where(x => x.CreateBy == _userService.UserName)
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreateTime)
                    .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
            if (string.IsNullOrEmpty(request.FilterData.TransactionTypeId) == false)
            {
                query = query.Where(x => x.TransactionTypeId == request.FilterData.TransactionTypeId);
            }
            if (request.FilterData.ListStatus != null && request.FilterData.ListStatus.Count > 0)
            {
                if (request.FilterData.ListStatus.Contains(PropertyApproveStatus.Draft)) 
                {
                    query = query.Where(x => request.FilterData.ListStatus.Contains(x.IsApprove) || x.IsTemp == true);
                }
                else
                {
                    query = query.Where(x => request.FilterData.ListStatus.Contains(x.IsApprove) && x.IsTemp == false);
                }
            }
            if (request.FilterData.FromDate != null)
            {
                DateTime fromDate = request.FilterData.FromDate.Value.Date;
                query = query.Where(x => x.ApproveDate != null && x.ApproveDate.Value.Date >= fromDate);
            }
            if (request.FilterData.ToDate != null)
            {
                DateTime toDate = request.FilterData.ToDate.Value.Date;
                /*
                query = query
                    .Where(x => (x.CreateTime.Date >= request.FilterData.FromDate.Value.Date && x.CreateTime.Date <= request.FilterData.ToDate.Value.Date)
                            && (x.IsApprove != PropertyApproveStatus.Active ||
                            (x.ExpiredDate.Value.Date >= request.FilterData.FromDate.Value.Date && x.ExpiredDate.Value.Date <= request.FilterData.ToDate.Value.Date)));
                */
                query = query.Where(x => x.ExpiredDate != null && x.ExpiredDate.Value.Date <= toDate);
            }
            

            var properties = await query.ToListAsync();

            properties = await _handlePropertyService.JoinPropertyElements(properties);

            return properties;
        }
    }
}
