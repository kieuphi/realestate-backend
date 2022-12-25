using MediatR;
using System;
using System.Collections.Generic;
using Models = General.Domain.Models;
using System.Threading.Tasks;
using General.Application.Interfaces;
using AutoMapper;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace General.Application.ImageCategory.Queries
{
    public class GetAllImageCategoryQuery : IRequest<List<Models.ImageCategoryModel>>
    {
    }

    public class GetAllImageCategoryQueryHandler : IRequestHandler<GetAllImageCategoryQuery, List<Models.ImageCategoryModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllImageCategoryQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<Models.ImageCategoryModel>> Handle(GetAllImageCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.ImageCategory.AsNoTracking().ToListAsync();
            return await Task.FromResult(_mapper.Map<List<Models.ImageCategoryModel>>(result));
        }
    }
}
