using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Project.Queries
{
    public class GetAllProjectQuery : IRequest<List<ProjectModel>>
    {
    }

    public class GetAllProjectQueryHandler : IRequestHandler<GetAllProjectQuery, List<ProjectModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetAllProjectQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<List<ProjectModel>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var result = await _context.Project
                               .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == ProjectApproveStatus.Active)
                               .AsNoTracking()
                               .OrderByDescending(x => x.CreateTime)
                               .ProjectTo<ProjectModel>(_mapper.ConfigurationProvider).ToListAsync();

            if(result.Count() > 0)
            {
                for(int i = 0; i < result.Count(); i++)
                {
                    result[i].CoverImageUrl = !string.IsNullOrEmpty(result[i].CoverImage) ? host + result[i].CoverImage : "";
                    result[i].ProjectLogoUrl = !string.IsNullOrEmpty(result[i].ProjectLogo) ? host + result[i].ProjectLogo : "";
                }
            }

            return result;
        }
    }
}
