using AutoMapper;
using AutoMapper.QueryableExtensions;
using General.Application.Interfaces;
using General.Domain.Models.PropertyElementModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.PropertyMeetingNote.Queries
{
    public class GetPropertyMeetingNoteByPropertyIdQuery : IRequest<PropertyMeetingNoteModel>
    {
        public Guid PropertyId { set; get; }
    }

    public class GetPropertyMeetingNoteByPropertyIdQueryHandler : IRequestHandler<GetPropertyMeetingNoteByPropertyIdQuery, PropertyMeetingNoteModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetPropertyMeetingNoteByPropertyIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PropertyMeetingNoteModel> Handle(GetPropertyMeetingNoteByPropertyIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.PropertyMeetingNote
                            .Where(x => x.PropertyId == request.PropertyId)
                            .AsNoTracking()
                            .ProjectTo<PropertyMeetingNoteModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            return result;
        }
    }
}
