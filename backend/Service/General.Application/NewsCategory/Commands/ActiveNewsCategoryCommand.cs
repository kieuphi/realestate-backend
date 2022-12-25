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
    public class ActiveNewsCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class ActiveNewsCategoryCommandHandler : IRequestHandler<ActiveNewsCategoryCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ActiveNewsCategoryCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(ActiveNewsCategoryCommand request, CancellationToken cancellationToken)
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

            if (entity.IsApprove == NewsApproveStatus.Active)
            {
                return Result.Failure("This news category has been active!");
            }

            entity.IsApprove = NewsApproveStatus.Active;
            entity.ApproveDate = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
