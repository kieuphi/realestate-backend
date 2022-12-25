using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;

namespace General.Application.Profile.Commands
{
    public class DeleteProfileCommand : IRequest<Result>
    {
        public Guid ProfileId { set; get; }
    }

    public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public DeleteProfileCommandHandler(
            IApplicationDbContext context,
            IIdentityService identityService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _context.ProfileInformation.FindAsync(request.ProfileId);
            var user = await _identityService.GetUserByIdentifierAsync(profile.UserId.ToString());

            if (profile == null)
            {
                return Result.Failure(new List<string> { "The specified profile not exists." });
            } else
            {
                if (user != null)
                {
                    user.IsDelete = true;
                }

                profile.IsDeleted = DeletedStatus.True;
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
