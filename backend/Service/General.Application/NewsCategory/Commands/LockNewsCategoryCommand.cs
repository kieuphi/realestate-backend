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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace General.Application.NewsCategory.Commands
{
    public class LockNewsCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class LockNewsCategoryCommandHandler : IRequestHandler<LockNewsCategoryCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public LockNewsCategoryCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(LockNewsCategoryCommand request, CancellationToken cancellationToken)
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

            entity.IsApprove = NewsApproveStatus.Lock;

            var news = await _context.News.Where(x => x.CategoryId == request.Id).ToListAsync();
            if (news.Count() > 0)
            {
                for (int i = 0; i < news.Count(); i++)
                {
                    news[i].IsApprove = NewsApproveStatus.Lock;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
