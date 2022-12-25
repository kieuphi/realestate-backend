using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Interfaces;
using General.Domain.Enums;
using Common.Shared.Enums;

namespace General.Application.News.Commands
{
    public class UnLockNewsCommand : IRequest<Result>
    {
        public Guid NewsId { set; get; }
    }

    public class UnLockNewsCommandHandler : IRequestHandler<UnLockNewsCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public UnLockNewsCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnLockNewsCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.News.FindAsync(request.NewsId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified news not exists." });
            }

            if (entity.IsApprove == NewsApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This news has been unLocked!" });
            }

            entity.IsApprove = NewsApproveStatus.InActive;
            entity.IsDeleted = DeletedStatus.False;
            entity.ApproveDate = null;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
