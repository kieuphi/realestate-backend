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

namespace General.Application.PropertyNearest.Commands
{
    public class CreatePropertyNearestCommand : IRequest<Result>
    {
        public CreatePropertyNearestModel Model { set; get; }
    }

    public class CreatePropertyNearestCommandHandler : IRequestHandler<CreatePropertyNearestCommand, Result>
    {
        private readonly IAsyncRepository<PropertyNearestEntity> _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public CreatePropertyNearestCommandHandler(
            IAsyncRepository<PropertyNearestEntity> repository,
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


        public async Task<Result> Handle(CreatePropertyNearestCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var idExisted = await _context.PropertyNearest.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (idExisted != null)
            {
                return Result.Failure($"The specified Property Nearest is invalid: {newId}");
            }
            
            var propertyNearest = await _context.PropertyNearest.Where(x => x.PropertyId == model.PropertyId).FirstOrDefaultAsync();
            if (propertyNearest != null)
            {
                _context.PropertyNearest.Remove(propertyNearest);
            }

            PropertyNearestEntity entity = new PropertyNearestEntity()
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
