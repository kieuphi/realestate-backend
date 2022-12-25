using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Contact.Queries
{
    public class GetAllContactQuery : IRequest<List<ContactModel>>
    {
    }

    public class GetAllContactQueryHandler : IRequestHandler<GetAllContactQuery, List<ContactModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllContactQueryHandler(
          IMapper mapper,
          IApplicationDbContext context
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ContactModel>> Handle(GetAllContactQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Contact
                              .Where(x => x.IsDeleted == DeletedStatus.False)
                              .AsNoTracking()
                              .OrderByDescending(x => x.CreateTime)
                              .ProjectTo<ContactModel>(_mapper.ConfigurationProvider).ToListAsync();

            return result;
        }
    }
}
