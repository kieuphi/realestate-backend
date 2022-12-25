using AutoMapper;
using System;
using Models = General.Domain.Models;
using Entities = General.Domain.Entities;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Models;
using Common.Shared.Models;

namespace General.Application.AttachmentType.Commands
{
    public class AddAttachmentTypeCommand : IRequest<Result>
    {
        public Models.AttachmentTypeModel Model { get; set; }
    }

    public class AddAttachmentTypeCommandHandler : IRequestHandler<AddAttachmentTypeCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AddAttachmentTypeCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(AddAttachmentTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Entities.AttachmentTypeEntity>(request.Model);
            _context.AttachmentType.Add(entity);

            return await _context.SaveChangesAsync() > 0 ? Result.Success() : Result.Failure("Failed to add attachment type");
        }
    }
}
