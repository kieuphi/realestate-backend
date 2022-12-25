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
    public class CreateSavedSearchCommand : IRequest<Result>
    {
        public CreateUserSavedSearchModel CreateUserSavedSearchModel { get; set; }
    }

    public class CreateSavedSearchCommandHandler : IRequestHandler<CreateSavedSearchCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public CreateSavedSearchCommandHandler(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(CreateSavedSearchCommand request, CancellationToken cancellationToken)
        {
            var createModel = request.CreateUserSavedSearchModel;
            if (string.IsNullOrEmpty(createModel.Name))
            {
                return Result.Failure("UserSavedSearchNameIsRequire");
            }
            //check exists
            var existData = await _context.UserSavedSearch
                .Where(x => x.UserId == Guid.Parse(_userService.UserId) && x.Name == createModel.Name.Trim())
                .FirstOrDefaultAsync();
            if (existData != null)
            {
                return Result.Failure("UserSavedSearchNameIsExist");
            }

            UserSavedSearchEntity entity = new UserSavedSearchEntity
            {
                UserId = Guid.Parse(_userService.UserId),
                Name = createModel.Name.Trim(),
                Keyword = createModel.Keyword.Trim(),
                Type = createModel.Type
            };

            _context.UserSavedSearch.Add(entity);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
