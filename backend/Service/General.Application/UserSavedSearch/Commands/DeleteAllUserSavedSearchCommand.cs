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
    public class DeleteAllUserSavedSearchCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAllUserSavedSearchCommandHandler : IRequestHandler<DeleteAllUserSavedSearchCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public DeleteAllUserSavedSearchCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(DeleteAllUserSavedSearchCommand request, CancellationToken cancellationToken)
        {
            var listDeleteData = await _context.UserSavedSearch.Where(x => x.UserId == Guid.Parse(_userService.UserId)).ToListAsync();
            _context.UserSavedSearch.RemoveRange(listDeleteData);

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
