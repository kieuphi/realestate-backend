using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;

namespace General.Application.Banner.Command
{
    public class DeleteBannerCommand : IRequest<Result>
    {
        public Guid Id { set; get; }
    }

    public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteBannerCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Banner.FindAsync(request.Id);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Banner not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
