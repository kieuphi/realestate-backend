using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Enums;

namespace General.Application.Project.Commands
{
    public class PostProjectCommand : IRequest<Result>
    {
        public Guid ProjectId { set; get; }
    }

    public class PostProjectCommandHandler : IRequestHandler<PostProjectCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public PostProjectCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(PostProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Project.FindAsync(request.ProjectId);

            if (entity == null)
            {
                return Result.Failure("The specified projectId not exists." );
            }

            if (entity.IsApprove == ProjectApproveStatus.Lock)
            {
                return Result.Failure("This project has been locked!");
            }

            if (entity.IsApprove == ProjectApproveStatus.Active)
            {
                return Result.Failure("This project has been posted!");
            }

            entity.IsApprove = ProjectApproveStatus.Active;
            entity.ApproveDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
