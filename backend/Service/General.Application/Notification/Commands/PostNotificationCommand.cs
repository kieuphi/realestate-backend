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
    public class PostNotificationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class PostNotificationCommandHandler : IRequestHandler<PostNotificationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public PostNotificationCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(PostNotificationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Notification.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return Result.Failure("IdInvalid");
            }

            entity.IsPosted = true;
            entity.PostedTime = DateTime.Now;
            entity.PostedBy = _userService.UserName;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
