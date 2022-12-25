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
    public class GetCountNotificationByUserQuery : IRequest<int>
    {
        public bool? IsSeen { get; set; }
    }

    public class GetCountNotificationByUserQueryHandler : IRequestHandler<GetCountNotificationByUserQuery, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetCountNotificationByUserQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<int> Handle(GetCountNotificationByUserQuery request, CancellationToken cancellationToken)
        {
            List<Guid> listNotificationRemoveId = await _context.UserNotificationRemove
                .Where(x => x.UserId == Guid.Parse(_userService.UserId))
                .Select(x => x.NotificationId)
                .ToListAsync();

            var query = _context.Notification
               .Where(x => x.IsPosted == true && listNotificationRemoveId.Contains(x.Id) == false);

            if (request.IsSeen != null)
            {
                List<Guid> listNotificationSeenId = await _context.UserNotificationSeen
                    .Where(x => x.UserId == Guid.Parse(_userService.UserId))
                    .Select(x => x.NotificationId)
                    .ToListAsync();
                if (request.IsSeen == true)
                {
                    query = query.Where(x => listNotificationSeenId.Contains(x.Id));
                }
                else
                {
                    query = query.Where(x => listNotificationSeenId.Contains(x.Id) == false);
                }
            }

            return await query.CountAsync();
        }
    }
}
