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
using General.Application.NewsCategory.Queries;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace General.Application.NewsCategory.Commands
{
    public class CreateNewsCategoryCommand : IRequest<Result>
    {
        public CreateNewsCategoryModel Model { set; get; }
    }

    public class CreateNewsCategoryCommandHandler : IRequestHandler<CreateNewsCategoryCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<NewsCategoryEntity> _repository;
        private readonly ILogger<CreateNewsCategoryCommandHandler> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public CreateNewsCategoryCommandHandler(
            IMapper mapper,
            IAsyncRepository<NewsCategoryEntity> repository,
            ILogger<CreateNewsCategoryCommandHandler> logger,
            IApplicationDbContext context,
            IMediator mediator
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(CreateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var newsCategory = await _context.NewsCategory.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (newsCategory != null)
            {
                return Result.Failure($"The specified News Category is invalid: {newId}");
            }
            //check exists
            var existData = await _context.NewsCategory
                .Where(x => x.CategoryNameVi == model.CategoryNameVi.Trim() || x.CategoryNameEn== model.CategoryNameEn.Trim())
                .FirstOrDefaultAsync();
            if (existData != null)
            {
                return Result.Failure("The specified News Category already exist");
            }

            NewsCategoryEntity entity = new NewsCategoryEntity()
            {
                Id = newId,
                CategoryNameVi = model.CategoryNameVi.Trim(),
                CategoryNameEn = model.CategoryNameEn.Trim(),
                IsApprove = NewsApproveStatus.New
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());

            var result = await _mediator.Send(new GetNewsCategoryByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }
    }
}
