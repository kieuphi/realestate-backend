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

namespace General.Application.News.Commands
{
    public class PostNewsCommand : IRequest<Result>
    {
        public Guid NewsId { set; get; }
    }

    public class PostNewsCommandHandler : IRequestHandler<PostNewsCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public PostNewsCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(PostNewsCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.News.FindAsync(request.NewsId);

            if (entity == null)
            {
                return Result.Failure("The specified news not exists.");
            }

            if (entity.IsApprove == NewsApproveStatus.Lock)
            {
                return Result.Failure("This news has been locked!");
            }

            if (entity.IsApprove == NewsApproveStatus.Active)
            {
                return Result.Failure("This news has been posted!");
            }

            entity.IsApprove = NewsApproveStatus.Active;
            entity.ApproveDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
