using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Enums;

namespace General.Application.Property.Commands
{
    public class UnPostPropertyCommand : IRequest<Result>
    {
        public Guid PropertyId { set; get; }
    }

    public class UnPostPropertyCommandHandler : IRequestHandler<UnPostPropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UnPostPropertyCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnPostPropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }

            if (entity.IsApprove == PropertyApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This property has been unposted!" });
            }

            entity.IsApprove = PropertyApproveStatus.InActive;
            entity.ApproveDate = null;
            entity.TimeForPostId = null;
            entity.ExpiredDate = null;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
