using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Common.Shared.Enums;

namespace General.Application.Contact.Commands
{
    public class DeleteContactCommand : IRequest<Result>
    {
        public Guid ContactId { set; get;}
    }

    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteContactCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contact.FindAsync(request.ContactId);

            if (contact == null)
            {
                return Result.Failure(new List<string> { "The specified Contact not exists." });
            }

            contact.IsDeleted = DeletedStatus.True;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
