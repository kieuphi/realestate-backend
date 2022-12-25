using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;

namespace General.Application.TimeForPost.Commands
{
    public class DeleteTimeForPostCommand : IRequest<Result>
    {
        public Guid TimeForPostId { set; get; }
    }

    public class DeleteTimeForPostCommandHandler : IRequestHandler<DeleteTimeForPostCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTimeForPostCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteTimeForPostCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TimeForPost.FindAsync(request.TimeForPostId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Time For Post not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
