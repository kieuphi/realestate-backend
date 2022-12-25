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
    public class UnPostProjectCommand : IRequest<Result>
    {
        public Guid ProjectId { set; get; }
    }

    public class UnPostProjectCommandHandler : IRequestHandler<UnPostProjectCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UnPostProjectCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnPostProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Project.FindAsync(request.ProjectId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified projectId not exists." });
            }

            if (entity.IsApprove == ProjectApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This project has been unposted!" });
            }

            entity.IsApprove = ProjectApproveStatus.InActive;
            entity.ApproveDate = null;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
