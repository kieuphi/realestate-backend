using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Application.News.Queries;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace General.Application.News.Commands
{
    public class CreateNewsCommand : IRequest<Result>
    {
        public CreateNewsModel Model { set; get; }
    }

    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<NewsEntity> _repository;
        private readonly ILogger<CreateNewsCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICommonFunctionService _commonFunctionService;

        public CreateNewsCommandHandler(
            IMapper mapper,
            IAsyncRepository<NewsEntity> repository,
            ILogger<CreateNewsCommandHandler> logger,
            IApplicationDbContext context,
            IMediator mediator,
            ICommonFunctionService commonFunctionService
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var news = await _context.News.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (news != null)
            {
                return Result.Failure($"The specified News is invalid: {newId}");
            }

            NewsEntity entity = new NewsEntity()
            {
                Id = newId,
                TitleVi = model.TitleVi,
                TitleEn = model.TitleEn,
                Slug = _commonFunctionService.GenerateFriendlyUrl(model.TitleEn, 0),

                Keyword = model.Keyword,

                ContentVi = model.ContentVi,
                ContentEn = model.ContentEn,

                DescriptionsVi = model.DescriptionsVi,
                DescriptionsEn = model.DescriptionsEn,

                CategoryId = model.CategoryId,
                ImageUrl = model.ImageUrl,
                Featured = model.Featured ?? false,
                IsHotNews = model.IsHotNews ?? false,
                IsWellRead = model.IsWellRead,
                Position = model.Position,
                IsApprove = NewsApproveStatus.New
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetNewsByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }
    }
}
