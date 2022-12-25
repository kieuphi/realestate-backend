using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;
using General.Domain.Enums;

namespace General.Application.Property.Commands
{
    public class DeletePropertyCommand : IRequest<Result>
    {
        public Guid PropertyId { set; get; }
    }

    public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeletePropertyCommandHandler(
            IApplicationDbContext context
        ) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            entity.IsApprove = PropertyApproveStatus.Lock;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
