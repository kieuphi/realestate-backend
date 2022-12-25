using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Application.TimeForPost.Queries;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace General.Application.TimeForPost.Commands
{
    public class CreateTimeForPostCommand : IRequest<Result>
    {
        public CreateTimeForPostModel Model { set; get; }
    }

    public class CreateTimeForPostCommandHandler : IRequestHandler<CreateTimeForPostCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<TimeForPostEntity> _repository;
        private readonly ILogger<CreateTimeForPostCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateTimeForPostCommandHandler(
            IMapper mapper,
            IAsyncRepository<TimeForPostEntity> repository,
            ILogger<CreateTimeForPostCommandHandler> logger,
            IApplicationDbContext context,
            IMediator mediator
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }



        public async Task<Result> Handle(CreateTimeForPostCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var timeForPost = await _context.TimeForPost.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (timeForPost != null)
            {
                return Result.Failure($"The specified Time For Post is invalid: {newId}");
            }

            TimeForPostEntity entity = new TimeForPostEntity()
            {
                Id = newId,
                Value = model.Value,
                DisplayName = model.DisplayName,
                Description = model.Description
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetTimeForPostByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }
    }
}
