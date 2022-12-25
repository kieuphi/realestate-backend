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
    public class UpdateNotificationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public UpdateNotificationModel UpdateNotificationModel { get; set; }
    }

    public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public UpdateNotificationCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
        {
            var updateModel = request.UpdateNotificationModel;
            var entity = await _context.Notification.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return Result.Failure("The id is invalid");
            }

            if (string.IsNullOrEmpty(updateModel.Title))
            {
                return Result.Failure("The notification title is require");
            }
            if (string.IsNullOrEmpty(updateModel.Content))
            {
                return Result.Failure("The notification content is require");
            }
            if (entity.IsPosted == true)
            {
                return Result.Failure("Cannot edit notification was posted");
            }
            if (string.IsNullOrEmpty(updateModel.Link) == false && Uri.IsWellFormedUriString(updateModel.Link, UriKind.Absolute) == false)
            {
                return Result.Failure("The link is invalid");
            }

            //check exists
            var existData = await _context.Notification
                .Where(x => x.Title == updateModel.Title && x.Id != entity.Id)
                .FirstOrDefaultAsync();
            if (existData != null)
            {
                return Result.Failure("The notification title is exist");
            }

            entity.Title = updateModel.Title;
            entity.TitleVi = updateModel.TitleVi;
            entity.Content = updateModel.Content;
            entity.ContentVi = updateModel.ContentVi;
            entity.Link = updateModel.Link;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
