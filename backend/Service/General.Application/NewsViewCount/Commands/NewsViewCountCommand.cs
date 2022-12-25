using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using General.Domain.Entities;

namespace General.Application.NewsViewCount.Commands
{
    public class CountViewNewsCommand : IRequest<Result>
    {
        public Guid NewsId { set; get; }
    }

    public class CountViewNewsCommandHandler : IRequestHandler<CountViewNewsCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public CountViewNewsCommandHandler (
            IMapper mapper,
            IApplicationDbContext context
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(CountViewNewsCommand request, CancellationToken cancellationToken)
        {
            var newsId = request.NewsId;
            var newsView = await _context.NewsViewCount.Where(x => x.NewsId == newsId).FirstOrDefaultAsync();

            if (newsView == null)
            {
                var newId = Guid.NewGuid();

                _context.NewsViewCount.Add(new NewsViewCountEntity
                {
                    Id = Guid.NewGuid(),
                    NewsId = newsId,
                    ViewCount = 1
                });
            }
            else
            {
                newsView.ViewCount = newsView.ViewCount + 1;
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
