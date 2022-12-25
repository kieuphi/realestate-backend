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

namespace General.Application.NewsCategory.Commands
{
    public class PostNewsCategoryCommand : IRequest<Result>
    {
        public Guid NewsCategoryId { set; get; }
    }

    public class PostNewsCategoryCommandHandler : IRequestHandler<PostNewsCategoryCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public PostNewsCategoryCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(PostNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NewsCategory.FindAsync(request.NewsCategoryId);

            if (entity == null)
            {
                return Result.Failure("The specified news category not exists.");
            }

            if (entity.IsApprove == NewsApproveStatus.Lock)
            {
                return Result.Failure("This news category has been locked!");
            }

            if (entity.IsApprove == NewsApproveStatus.Active)
            {
                return Result.Failure("This news category has been posted!");
            }

            entity.IsApprove = NewsApproveStatus.Active;
            entity.ApproveDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
