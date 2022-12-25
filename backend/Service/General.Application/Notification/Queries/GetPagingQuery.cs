using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Notification.Queries
{
    public class GetPagingQuery : IRequest<PaginatedList<NotificationModel>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetPagingQueryHandler : IRequestHandler<GetPagingQuery, PaginatedList<NotificationModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetPagingQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PaginatedList<NotificationModel>> Handle(GetPagingQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Notification
                .ProjectTo<NotificationModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(x => x.PostedTime);
            var data = await PaginatedList<NotificationModel>.CreateAsync(query,request.PageNumber,request.PageSize);

            return data;
        }
    }
}
