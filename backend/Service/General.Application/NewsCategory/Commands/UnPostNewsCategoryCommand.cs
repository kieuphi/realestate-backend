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
    public class UnPostNewsCategoryCommand : IRequest<Result>
    {
        public Guid NewsCategoryId { set; get; }
    }

    public class UnPostNewsCategoryCommandHandler : IRequestHandler<UnPostNewsCategoryCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public UnPostNewsCategoryCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(UnPostNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NewsCategory.FindAsync(request.NewsCategoryId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified news category not exists." });
            }

            if (entity.IsApprove == NewsApproveStatus.InActive)
            {
                return Result.Failure(new List<string> { "This news category has been unposted!" });
            }

            entity.IsApprove = NewsApproveStatus.InActive;
            entity.ApproveDate = null;

            var news = await _context.News.Where(x => x.CategoryId == request.NewsCategoryId).ToListAsync();
            if (news.Count() > 0)
            {
                for (int i = 0; i < news.Count(); i++)
                {
                    news[i].IsApprove = NewsApproveStatus.InActive;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
