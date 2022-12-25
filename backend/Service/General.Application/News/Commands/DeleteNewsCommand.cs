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

namespace General.Application.News.Commands
{
    public class DeleteNewsCommand : IRequest<Result>
    {
        public Guid NewsId { set; get; }
    }

    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteNewsCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.News.FindAsync(request.NewsId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified News not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            entity.IsApprove = NewsApproveStatus.Lock;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
