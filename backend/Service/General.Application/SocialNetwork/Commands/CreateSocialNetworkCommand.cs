using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Common.Interfaces;
using General.Domain.Entities;
using Microsoft.Extensions.Logging;
using General.Application.Project.Commands;
using General.Application.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using General.Application.SocialNetwork.Queries;

namespace General.Application.SocialNetwork.Commands
{
    public class CreateSocialNetworkCommand : IRequest<Result>
    {
        public CreateSocialNetworkModel Model { set; get; }
    }

    public class CreateSocialNetworkCommandHandler : IRequestHandler<CreateSocialNetworkCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<SocialNetworkEntity> _repository;
        private readonly ILogger<CreateSocialNetworkModel> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateSocialNetworkCommandHandler(
            IMapper mapper,
            IAsyncRepository<SocialNetworkEntity> repository,
            ILogger<CreateSocialNetworkModel> logger,
            IApplicationDbContext context,
            IMediator mediator
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(CreateSocialNetworkCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var socialNetwork = await _context.SocialNetwork.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (socialNetwork != null)
            {
                return Result.Failure($"The specified Social Network is invalid: {newId}");
            }

            SocialNetworkEntity entity = new SocialNetworkEntity()
            {
                Id = newId,
                AppName = model.AppName,
                ICon = model.ICon,
                Descriptions = model.Descriptions,
                IsShowFooter = model.IsShowFooter
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetSocialNetworkByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }
    }
}
