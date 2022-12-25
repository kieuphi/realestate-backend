using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Notification.Commands
{
    public class SeenNotificationByUserCommand : IRequest<Result>
    {
        public List<Guid> listId { get; set; }
    }

    public class SeenNotificationByUserCommandHandler : IRequestHandler<SeenNotificationByUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public SeenNotificationByUserCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(SeenNotificationByUserCommand request, CancellationToken cancellationToken)
        {
            List<NotificationEntity> listNotification = await _context.Notification
                .Where(x => x.IsPosted == true && request.listId.Contains(x.Id))
                .ToListAsync();
            
            for (int i = 0; i < listNotification.Count; i++)
            {
                UserNotificationSeenEntity entity = new UserNotificationSeenEntity
                {
                    UserId = Guid.Parse(_userService.UserId),
                    NotificationId = listNotification[i].Id
                };
                _context.UserNotificationSeen.Add(entity);
            }
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
