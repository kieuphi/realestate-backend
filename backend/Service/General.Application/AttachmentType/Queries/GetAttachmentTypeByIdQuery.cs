using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Models = General.Domain.Models;
using General.Application.Interfaces;

namespace General.Application.AttachmentType.Queries
{
    public class GetAttachmentTypeByIdQuery : IRequest<Models.AttachmentTypeModel>
    {
        public Guid Id;
    }

    public class GetAttachmentTypeByIdQueryHandler : IRequestHandler<GetAttachmentTypeByIdQuery, Models.AttachmentTypeModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAttachmentTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Models.AttachmentTypeModel> Handle(GetAttachmentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var entiy = _context.AttachmentType.Find(request.Id);

            if (entiy == null)
            {
                throw new ArgumentNullException(nameof(entiy));
            }

            return await Task.FromResult(_mapper.Map<Models.AttachmentTypeModel>(entiy));
        }
    }
}
