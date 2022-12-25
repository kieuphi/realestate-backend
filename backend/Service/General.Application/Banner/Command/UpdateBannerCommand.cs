using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.Shared.Enums;

namespace General.Application.Banner.Command
{
    public class UpdateBannerCommand : IRequest<Result>
    {
        public Guid BannerId { set; get; }
        public CreateBannerModel Model { set; get; }
    }

    public class UpdateBannerCommandHandler : IRequestHandler<UpdateBannerCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UpdateBannerCommandHandler(
            IApplicationDbContext context,
            IMediator mediator
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(UpdateBannerCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.Banner.FindAsync(request.BannerId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Banner not exists." });
            }

            var order = await _context.Banner
                        .Where(x => x.BannerOrder == model.BannerOrder 
                            && x.Id != request.BannerId 
                            && x.IsDeleted == DeletedStatus.False
                            && x.BannerType == model.BannerType)
                        .ToListAsync();

            if (order.Count() > 0)
            {
                return Result.Failure($"This order number was existed: {model.BannerOrder}");
            }

            if(model.BannerOrder == null || model.BannerOrder <= 0)
            {
                return Result.Failure($"This order number must be greater than 0");
            }

            entity.BannerName = model.BannerName;
            entity.BannerType = model.BannerType;
            entity.ImageUrl = model.ImageUrl;
            entity.Descriptions = model.Descriptions;
            entity.BannerOrder = model.BannerOrder;
            entity.BannerOrder = model.BannerOrder != null ? model.BannerOrder : 0;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
