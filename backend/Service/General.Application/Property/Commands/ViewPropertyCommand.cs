using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Models;
using General.Domain.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Property.Commands
{
    public class ViewPropertyCommand : IRequest<Result>
    {
        public Guid PropertyId { get; set; }
    }

    public class ViewPropertyCommandHandler : IRequestHandler<ViewPropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public ViewPropertyCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService userService
        ) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(ViewPropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified PropertyId not exists." });
            }

            if (entity.IsApprove != PropertyApproveStatus.Active)
            {
                return Result.Failure(new List<string> { "This property is unpost" });
            }

            var viewCountEntity = await _context.PropertyViewCount.Where(x => x.PropertyId == request.PropertyId).FirstOrDefaultAsync();
            int userLoginViewCount = 0;
            int unLoginViewCount = 0;

            if (string.IsNullOrEmpty(_userService.UserName))
            {
                unLoginViewCount = 1;
            }
            else
            {
                userLoginViewCount = 1;
            }

            if (viewCountEntity == null)
            {
                viewCountEntity = new Domain.Entities.PropertyViewCountEntity
                {
                    PropertyId = request.PropertyId,
                    ViewCount = 1,
                    UserLoginViewCount = userLoginViewCount,
                    UnLoginViewCount = unLoginViewCount
                };
                _context.PropertyViewCount.Add(viewCountEntity);
            }
            else
            {
                viewCountEntity.ViewCount = viewCountEntity.ViewCount + 1;
                viewCountEntity.UserLoginViewCount = viewCountEntity.ViewCount + userLoginViewCount;
                viewCountEntity.UnLoginViewCount = viewCountEntity.ViewCount + unLoginViewCount;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
