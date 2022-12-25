using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Application.Common.Interfaces;

namespace General.Application.PropertyNearest.Commands
{
    public class DeletePropertyNearestCommand : IRequest<Result>
    {
        public Guid PropertyNearestId { set; get; }
    }

    public class DeletePropertyNearestCommandHandler : IRequestHandler<DeletePropertyNearestCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<PropertyNearestEntity> _repository;

        public DeletePropertyNearestCommandHandler(
            IAsyncRepository<PropertyNearestEntity> repository,
            IApplicationDbContext context
        ) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeletePropertyNearestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PropertyNearest.FindAsync(request.PropertyNearestId);

            if (entity == null)
            {
                return default;
            }

            _context.PropertyNearest.Remove(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
