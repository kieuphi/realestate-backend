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

namespace General.Application.PropertyFavorite.Commands
{
    public class DeletePropertyFavoriteCommand : IRequest<Result>
    {
        public Guid PropertyFavoriteId { set; get; }
    }

    public class DeletePropertyFavoriteCommandHandler : IRequestHandler<DeletePropertyFavoriteCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<PropertyFavoriteEntity> _repository;

        public DeletePropertyFavoriteCommandHandler(
            IAsyncRepository<PropertyFavoriteEntity> repository,
            IApplicationDbContext context
        ) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeletePropertyFavoriteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PropertyFavorite.FindAsync(request.PropertyFavoriteId);

            if (entity == null)
            {
                return default;
            }

            _context.PropertyFavorite.Remove(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
