using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;

namespace General.Application.News.Commands
{
    public class UpdateNewsCommand : IRequest<Result>
    {
        public CreateNewsModel Model { set; get; }
        public Guid NewsId { set; get; }
    }

    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateNewsCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.News.FindAsync(request.NewsId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified News not exists." });
            }

            entity.TitleVi = model.TitleVi;
            entity.TitleEn = model.TitleEn;

            entity.Keyword = model.Keyword;

            entity.ContentVi = model.ContentVi;
            entity.ContentEn = model.ContentEn;

            entity.DescriptionsVi = model.DescriptionsVi;
            entity.DescriptionsEn = model.DescriptionsEn;

            entity.CategoryId = model.CategoryId;
            entity.ImageUrl = model.ImageUrl;
            entity.Featured = model.Featured ?? false;
            entity.IsHotNews = model.IsHotNews ?? false;
            entity.IsWellRead = model.IsWellRead ?? false;
            entity.Position = model.Position;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
