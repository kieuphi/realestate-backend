using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;

namespace General.Application.Contact.Commands
{
    public class UpdateContactCommand : IRequest<Result>
    {
        public CreateContactModel Model { set; get; }
        public Guid ContactId { set; get; }
    }

    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateContactCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var contact = await _context.Contact.FindAsync(request.ContactId);

            if (contact == null)
            {
                return Result.Failure(new List<string> { "The specified Contact not exists." });
            }

            contact.Subject = model.Subject;
            contact.Name = model.Name;
            contact.Email = model.Email;
            contact.Phone = model.Phone;
            contact.Message = model.Message;
            contact.ContactType= model.ContactType;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
