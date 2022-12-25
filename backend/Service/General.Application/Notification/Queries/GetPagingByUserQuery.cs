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
    public class GetPagingByUserQuery : IRequest<PaginatedList<NotificationUserModel>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetPagingByUserQueryHandler : IRequestHandler<GetPagingByUserQuery, PaginatedList<NotificationUserModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetPagingByUserQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PaginatedList<NotificationUserModel>> Handle(GetPagingByUserQuery request, CancellationToken cancellationToken)
        {
            List<Guid> listNotificationRemoveId = await _context.UserNotificationRemove
                .Where(x => x.UserId == Guid.Parse(_userService.UserId))
                .Select(x => x.NotificationId)
                .ToListAsync();
            List<Guid> listNotificationSeenId = await _context.UserNotificationSeen
                .Where(x => x.UserId == Guid.Parse(_userService.UserId))
                .Select(x => x.NotificationId)
                .ToListAsync();

            var listData = await _context.Notification
                .Where(x => x.IsPosted == true && listNotificationRemoveId.Contains(x.Id) == false)
                .ProjectTo<NotificationUserModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            for (int i = 0; i < listData.Count; i++)
            {
                listData[i].IsSeen = false;
                if (listNotificationSeenId.Contains(listData[i].Id) == true)
                {
                    listData[i].IsSeen = true;
                }
            }
            listData = listData.OrderBy(x => x.IsSeen).ThenByDescending(x => x.PostedTime).ToList();

            var data = PaginatedList<NotificationUserModel>.Create(listData, request.PageNumber,request.PageSize);

            return data;
        }
    }
}
