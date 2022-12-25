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
    public class CreateNotificationCommand : IRequest<Result>
    {
        public CreateNotificationModel CreateNotificationModel { get; set; }
    }

    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public CreateNotificationCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var createModel = request.CreateNotificationModel;
            if (string.IsNullOrEmpty(createModel.Title))
            {
                return Result.Failure("The notification title is require");
            }
            if (string.IsNullOrEmpty(createModel.Content))
            {
                return Result.Failure("The notification content is require");
            }
            if (string.IsNullOrEmpty(createModel.Link) == false && Uri.IsWellFormedUriString(createModel.Link, UriKind.Absolute) == false)
            {
                return Result.Failure("The link is invalid");
            }

            //check exists
            var existData = await _context.Notification
                .Where(x => x.Title == createModel.Title)
                .FirstOrDefaultAsync();
            if (existData != null)
            {
                return Result.Failure("The notification title is exist");
            }

            NotificationEntity entity = new NotificationEntity
            {
                Title = createModel.Title,
                TitleVi = createModel.TitleVi,
                Content = createModel.Content,
                ContentVi = createModel.ContentVi,
                Link = createModel.Link,
                IsPosted = false
            };
            if (createModel.IsPosted == true)
            {
                entity.IsPosted = true;
                entity.PostedTime = DateTime.Now;
                entity.PostedBy = _userService.UserName;
            }

            _context.Notification.Add(entity);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
