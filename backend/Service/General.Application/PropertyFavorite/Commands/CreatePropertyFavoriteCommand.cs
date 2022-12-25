using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.PropertyFavorite.Commands
{
    public class CreatePropertyFavoriteCommand : IRequest<Result>
    {
        public CreatePropertyFavoriteModel Model { set; get; }
    }

    public class CreatePropertyFavoriteCommandHandler : IRequestHandler<CreatePropertyFavoriteCommand, Result>
    {
        private readonly IAsyncRepository<PropertyFavoriteEntity> _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public CreatePropertyFavoriteCommandHandler(
            IAsyncRepository<PropertyFavoriteEntity> repository,
            IApplicationDbContext context,
            IMediator mediator,
            IIdentityService identityService
        )
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }


        public async Task<Result> Handle(CreatePropertyFavoriteCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var idExisted = await _context.PropertyFavorite.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (idExisted != null)
            {
                return Result.Failure($"The specified Property Nearest is invalid: {newId}");
            }
            
            var propertyFavorite = await _context.PropertyFavorite.Where(x => x.PropertyId == model.PropertyId).FirstOrDefaultAsync();
            if (propertyFavorite != null)
            {
                _context.PropertyFavorite.Remove(propertyFavorite);
            }

            PropertyFavoriteEntity entity = new PropertyFavoriteEntity()
            {
                Id = newId,
                UserId = model.UserId,
                PropertyId = model.PropertyId
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
