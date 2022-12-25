using AutoMapper;
using AutoMapper.QueryableExtensions;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Contact.Queries
{
    public class GetContactByIdQuery : IRequest<ContactModel>
    {
        public Guid Id { set; get; }
    }

    public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetContactByIdQueryHandler (
            IMapper mapper,
            IApplicationDbContext context
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ContactModel> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Contact
                        .Where(x => x.Id == request.Id)
                        .AsNoTracking()
                        .ProjectTo<ContactModel>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync();

            return result;
        }
    }
}
