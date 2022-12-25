using MediatR;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;
using System;
using System.Linq;
using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using General.Application.Banner.Query;
using General.Application.Common.Interfaces;
using Common.Shared.Enums;

namespace General.Application.Banner.Command
{
    public class CreateBannerCommand : IRequest<Result>
    {
        public CreateBannerModel Model { set; get; }
    }

    public class CreateBannerCommandHandler : IRequestHandler<CreateBannerCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAsyncRepository<BannerEntity> _repository;

        public CreateBannerCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IMediator mediator,
            IAsyncRepository<BannerEntity> repository
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result> Handle(CreateBannerCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var banner = await _context.Banner.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (banner != null)
            {
                return Result.Failure($"The specified Banner is invalid: {newId}");
            }

            var order = await _context.Banner
                .Where(x => x.IsDeleted == DeletedStatus.False && x.BannerOrder == model.BannerOrder && x.BannerType == model.BannerType)
                .ToListAsync();
            if(order.Count() > 0)
            {
                return Result.Failure($"This order number was existed: {model.BannerOrder}");
            }

            if (model.BannerOrder == null || model.BannerOrder <= 0)
            {
                return Result.Failure($"This order number must be greater than 0");
            }

            BannerEntity entity = new BannerEntity()
            {
                Id = newId,
                BannerName = model.BannerName,
                BannerType = model.BannerType,
                ImageUrl = model.ImageUrl,
                Descriptions = model.Descriptions,
                BannerOrder = model.BannerOrder != null ? model.BannerOrder : 0
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetBannerByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }
    }
}
