using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Enums;
using Common.Shared.Enums;

namespace General.Application.Property.Commands
{
    public class UnLockPropertyCommand : IRequest<Result>
    {
        public Guid PropertyId { set; get; }
    }

    public class UnLockPropertyCommandHandler : IRequestHandler<UnLockPropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UnLockPropertyCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnLockPropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }

            if (entity.IsApprove == PropertyApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This property has been unlocked!" });
            }

            entity.IsApprove = PropertyApproveStatus.InActive;
            entity.IsDeleted = DeletedStatus.False;
            entity.ApproveDate = null;
            entity.TimeForPostId = null;
            entity.ExpiredDate = null;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
