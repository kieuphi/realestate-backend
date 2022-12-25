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

namespace General.Application.PropertyHeart.Commands
{
    public class DeletePropertyHeartCommand : IRequest<Result>
    {
        public Guid PropertyHeartId { set; get; }
    }

    public class DeletePropertyHeartCommandHandler : IRequestHandler<DeletePropertyHeartCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<PropertyHeartEntity> _repository;

        public DeletePropertyHeartCommandHandler(
            IAsyncRepository<PropertyHeartEntity> repository,
            IApplicationDbContext context
        ) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeletePropertyHeartCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PropertyHeart.FindAsync(request.PropertyHeartId);

            if (entity == null)
            {
                return default;
            }

            _context.PropertyHeart.Remove(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
