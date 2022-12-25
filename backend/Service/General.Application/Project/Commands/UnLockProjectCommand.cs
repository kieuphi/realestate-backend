using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Enums;
using Common.Shared.Enums;

namespace General.Application.Project.Commands
{
    public class UnLockProjectCommand : IRequest<Result>
    {
        public Guid ProjectId { set; get; }
    }

    public class UnLockProjectCommandHandler : IRequestHandler<UnLockProjectCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UnLockProjectCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnLockProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Project.FindAsync(request.ProjectId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified projectId not exists." });
            }

            if (entity.IsApprove == ProjectApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This project has been unlocked!" });
            }

            entity.IsApprove = ProjectApproveStatus.InActive;
            entity.IsDeleted = DeletedStatus.False;
            entity.ApproveDate = null;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
