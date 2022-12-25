using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;
using General.Domain.Enums;

namespace General.Application.Project.Commands
{
    public class DeleteProjectCommand : IRequest<Result>
    {
        public Guid ProjectId { set; get; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteProjectCommandHandler(
            IApplicationDbContext context
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Project.FindAsync(request.ProjectId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Project not exists." });
            }
            //if (entity.IsTemp == false)
            //{
            //    return Result.Failure("Cannot delete the available project");
            //}

            entity.IsDeleted = DeletedStatus.True;
            entity.IsApprove = ProjectApproveStatus.Lock;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
