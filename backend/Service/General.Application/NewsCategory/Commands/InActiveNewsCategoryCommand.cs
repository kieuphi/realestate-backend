using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Shared.Models;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.NewsCategory.Commands
{
    public class InActiveNewsCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class InActiveNewsCategoryCommandHandler : IRequestHandler<InActiveNewsCategoryCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public InActiveNewsCategoryCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(InActiveNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NewsCategory.FindAsync(request.Id);

            if (entity == null)
            {
                return Result.Failure("The specified news category not exists.");
            }

            if (entity.IsApprove == NewsApproveStatus.Lock)
            {
                return Result.Failure("This news category has been locked!");
            }
            if (entity.IsApprove == NewsApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This news category has been In-Active!" });
            }

            entity.IsApprove = NewsApproveStatus.InActive;
            entity.ApproveDate = null;

            var news = await _context.News.Where(x => x.CategoryId == entity.Id).ToListAsync();
            if (news.Count > 0)
            {
                for (int i = 0; i < news.Count; i++)
                {
                    news[i].IsApprove = NewsApproveStatus.InActive;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
