using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;
using General.Domain.Enums;

namespace General.Application.Property.Commands
{
    public class DeleteTempPropertyCommand : IRequest<Result>
    {
        public Guid PropertyId { set; get; }
    }

    public class DeleteTempPropertyCommandHandler : IRequestHandler<DeleteTempPropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public DeleteTempPropertyCommandHandler(IApplicationDbContext context, ICurrentUserService userService) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(DeleteTempPropertyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Property.FindAsync(request.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }
            if(entity.IsTemp == false)
            {
                return Result.Failure("Cannot delete the available property");
            }
            if(entity.CreateBy != _userService.UserName)
            {
                return Result.Failure("Not privilege");
            }

            _context.Property.Remove(entity);
            
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
