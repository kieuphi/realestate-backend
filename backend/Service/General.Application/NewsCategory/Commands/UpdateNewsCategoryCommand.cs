using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Shared.Models;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.NewsCategory.Commands
{
    public class UpdateNewsCategoryCommand : IRequest<Result>
    {
        public CreateNewsCategoryModel Model { set; get; } 
        public Guid NewsCategoryId { set; get; }
    }

    public class UpdateNewsCategoryCommandHandler : IRequestHandler<UpdateNewsCategoryCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateNewsCategoryCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.NewsCategory.FindAsync(request.NewsCategoryId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified News Category not exists." });
            }

            //check exists
            var existData = await _context.NewsCategory
                .Where(x => (x.Id != entity.Id && x.CategoryNameVi == request.Model.CategoryNameVi.Trim())
                    || (x.Id != entity.Id && x.CategoryNameEn == request.Model.CategoryNameEn.Trim()))
                .FirstOrDefaultAsync();
            if (existData != null)
            {
                return Result.Failure("The specified News Category already exist");
            }

            entity.CategoryNameVi = request.Model.CategoryNameVi;
            entity.CategoryNameEn = request.Model.CategoryNameEn;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
