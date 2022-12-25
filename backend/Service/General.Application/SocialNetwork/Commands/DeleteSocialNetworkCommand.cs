using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;

namespace General.Application.SocialNetwork.Commands
{
    public class DeleteSocialNetworkCommand : IRequest<Result>
    {
        public Guid SocialNetworkId { set; get; }
    }

    public class DeleteSocialNetworkCommandHandler : IRequestHandler<DeleteSocialNetworkCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSocialNetworkCommandHandler(
            IApplicationDbContext context
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteSocialNetworkCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.SocialNetwork.FindAsync(request.SocialNetworkId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Social Network not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
