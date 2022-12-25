using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.UserSavedSearch.Commands
{
    public class DeleteUserSavedSearchCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserSavedSearchCommandHandler : IRequestHandler<DeleteUserSavedSearchCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public DeleteUserSavedSearchCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(DeleteUserSavedSearchCommand request, CancellationToken cancellationToken)
        {
            var deleteData = await _context.UserSavedSearch.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            _context.UserSavedSearch.Remove(deleteData);

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
