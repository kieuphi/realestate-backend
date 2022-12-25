using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Models;
using General.Domain.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Property.Commands
{
    public class ApprovePropertyCommand : IRequest<Result>
    {
        public ApprovePropertyModel Model { set; get; }
    }

    public class ApprovePropertyCommandHandler : IRequestHandler<ApprovePropertyCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public ApprovePropertyCommandHandler(
            IApplicationDbContext context
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(ApprovePropertyCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            var entity = await _context.Property.FindAsync(model.PropertyId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified propertyId not exists." });
            }

            if (entity.IsApprove == PropertyApproveStatus.Lock)
            {
                return Result.Failure(new List<string> { "This property has been locked!" });
            }

            if (entity.IsApprove == PropertyApproveStatus.Active)
            {
                return Result.Failure(new List<string> { "This property has been posted!" });
            }

            if (model.TimeForPostId == null || model.TimeForPostId == Guid.Empty)
            {
                return Result.Failure(new List<string> { "Time for post is required!" });
            }

            entity.IsApprove = PropertyApproveStatus.Active;
            entity.ApproveDate = DateTime.Now;
            entity.TimeForPostId = model.TimeForPostId;

            var timeForPost = await _context.TimeForPost.Where(x => x.Id == model.TimeForPostId).FirstOrDefaultAsync();
            if (timeForPost != null)
            {
                entity.ExpiredDate = entity.ApproveDate.Value.AddDays(Convert.ToDouble(timeForPost.Value));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
