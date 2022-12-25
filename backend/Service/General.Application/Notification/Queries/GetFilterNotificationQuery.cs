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
    public class GetFilterNotificationQuery : IRequest<PaginatedList<NotificationModel>>
    {
        public FilterNotificationModel FilterNotificationModel { get; set; }
    }

    public class GetFilterNotificationQueryHandler : IRequestHandler<GetFilterNotificationQuery, PaginatedList<NotificationModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetFilterNotificationQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PaginatedList<NotificationModel>> Handle(GetFilterNotificationQuery request, CancellationToken cancellationToken)
        {
            var filter = request.FilterNotificationModel;

            var query = _context.Notification
                .ProjectTo<NotificationModel>(_mapper.ConfigurationProvider);
            if (string.IsNullOrEmpty(filter.Keyword) == false)
            {
                query = query.Where(x => x.Title.Contains(filter.Keyword) || x.TitleVi.Contains(filter.Keyword));
            }
            if (filter.IsPosted != null)
            {
                query = query.Where(x => x.IsPosted == filter.IsPosted);
            }
            query = query.OrderByDescending(x => x.PostedTime);

            var data = await PaginatedList<NotificationModel>.CreateAsync(query, filter.PageNumber,filter.PageSize);

            return data;
        }
    }
}
