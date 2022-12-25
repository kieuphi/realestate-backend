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
    public class RemoveAllNotificationByUserCommand : IRequest<Result>
    {
    }

    public class RemoveAllNotificationByUserCommandHandler : IRequestHandler<RemoveAllNotificationByUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public RemoveAllNotificationByUserCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(RemoveAllNotificationByUserCommand request, CancellationToken cancellationToken)
        {
            List<UserNotificationRemoveEntity> listEntity = await _context.UserNotificationRemove
                .Where(x => x.UserId == Guid.Parse(_userService.UserId))
                .ToListAsync();
            List<NotificationEntity> listNotification = await _context.Notification
                .Where(x => x.IsPosted == true)
                .ToListAsync();

            for (int i = 0; i < listNotification.Count; i++)
            {
                UserNotificationRemoveEntity entity = listEntity.Where(x => x.NotificationId == listNotification[i].Id).FirstOrDefault();
                if (entity == null)
                {
                    entity = new UserNotificationRemoveEntity
                    {
                        UserId = Guid.Parse(_userService.UserId),
                        NotificationId = listNotification[i].Id
                    };
                    _context.UserNotificationRemove.Add(entity);
                }
            }
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
