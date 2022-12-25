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
    public class RemoveNotificationByUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class RemoveNotificationByUserCommandHandler : IRequestHandler<RemoveNotificationByUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public RemoveNotificationByUserCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(RemoveNotificationByUserCommand request, CancellationToken cancellationToken)
        {
            UserNotificationRemoveEntity entity = await _context.UserNotificationRemove
                .Where(x => x.UserId == Guid.Parse(_userService.UserId) && x.NotificationId == request.Id)
                .FirstOrDefaultAsync();
            NotificationEntity notificationEntity = await _context.Notification
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();
            if (notificationEntity == null)
            {
                return Result.Failure("IdInvalid");
            }

            if (entity == null)
            {
                entity = new UserNotificationRemoveEntity
                {
                    UserId = Guid.Parse(_userService.UserId),
                    NotificationId = request.Id
                };
                _context.UserNotificationRemove.Add(entity);
                await _context.SaveChangesAsync();
            }

            return Result.Success();
        }
    }
}
