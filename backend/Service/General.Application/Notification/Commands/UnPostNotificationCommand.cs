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
    public class UnPostNotificationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class UnPostNotificationCommandHandler : IRequestHandler<UnPostNotificationCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public UnPostNotificationCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(UnPostNotificationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Notification.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return Result.Failure("IdInvalid");
            }

            entity.IsPosted = false;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
