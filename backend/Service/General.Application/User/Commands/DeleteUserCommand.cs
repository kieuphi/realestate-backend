using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace General.Application.User.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public Guid UserId { set; get; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public DeleteUserCommandHandler(
            IApplicationDbContext context,
            IIdentityService identityService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var profile = await _context.ProfileInformation.Where(x => x.UserId == request.UserId).FirstOrDefaultAsync();
            var user = await _identityService.GetUserByIdentifierAsync(request.UserId.ToString());

            if (user == null)
            {
                return Result.Failure(new List<string> { "The specified user not exists." });
            } 
            else
            {
                var deleteResult = await _identityService.DeleteUserAsync(user);
                if(deleteResult.Succeeded == false)
                {
                    return deleteResult;
                }
                _context.ProfileInformation.Remove(profile);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
