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

namespace General.Application.PropertyHeart.Commands
{
    public class SendPropertyHeartCommand : IRequest<Result>
    {
        public CreatePropertyHeartModel Model { set; get; }
    }

    public class CreatePropertyHeartCommandHandler : IRequestHandler<SendPropertyHeartCommand, Result>
    {
        private readonly IAsyncRepository<PropertyHeartEntity> _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public CreatePropertyHeartCommandHandler(
            IAsyncRepository<PropertyHeartEntity> repository,
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


        public async Task<Result> Handle(SendPropertyHeartCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();
            
            var propertyHeart = await _context.PropertyHeart
                .Where(x => x.PropertyId == model.PropertyId && x.UserId == model.UserId)
                .FirstOrDefaultAsync();
            if (propertyHeart != null)
            {
                _context.PropertyHeart.Remove(propertyHeart);
            } else
            {
                PropertyHeartEntity entity = new PropertyHeartEntity()
                {
                    Id = newId,
                    UserId = model.UserId,
                    PropertyId = model.PropertyId
                };

                await _repository.AddAsync(entity);
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
