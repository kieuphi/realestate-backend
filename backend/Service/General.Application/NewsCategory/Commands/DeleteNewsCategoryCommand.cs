using MediatR;
using System;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using System.Collections.Generic;
using Common.Shared.Enums;
using General.Domain.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace General.Application.NewsCategory.Commands
{
    public class DeleteNewsCategoryCommand : IRequest<Result>
    {
        public Guid NewsCategoryId { set; get; }
    }

    public class DeleteNewsCategoryCommandHandler : IRequestHandler<DeleteNewsCategoryCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteNewsCategoryCommandHandler(
            IApplicationDbContext context
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(DeleteNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NewsCategory.FindAsync(request.NewsCategoryId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified News Category not exists." });
            }

            entity.IsDeleted = DeletedStatus.True;
            entity.IsApprove = NewsApproveStatus.Lock;

            var news = await _context.News.Where(x => x.CategoryId == request.NewsCategoryId).ToListAsync();
            if (news.Count() > 0)
            {
                for(int i = 0; i < news.Count(); i++)
                {
                    news[i].IsDeleted = DeletedStatus.True;
                    news[i].IsApprove = NewsApproveStatus.Lock;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
