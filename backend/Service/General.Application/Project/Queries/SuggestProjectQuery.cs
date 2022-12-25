using AutoMapper;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Project.Queries
{
    public class SuggestProjectQuery : IRequest<List<SuggestSearchProjectModel>>
    {
        public string Keyword { set; get; }
    }

    public class SuggestProjectQueryHandler : IRequestHandler<SuggestProjectQuery, List<SuggestSearchProjectModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConvertVietNameseService _convertVietNameseService;

        public SuggestProjectQueryHandler (
            IApplicationDbContext context,
            IMapper mapper,
            IConvertVietNameseService convertVietNameseService
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
        }

        public async Task<List<SuggestSearchProjectModel>> Handle(SuggestProjectQuery request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower().Trim();
                var result = new List<SuggestSearchProjectModel>();
                var projects = await _context.Project
                                    .Where(x => x.IsApprove == ProjectApproveStatus.Active 
                                        && x.IsDeleted == DeletedStatus.False
                                        && (x.ProjectVi.ToLower().Contains(keyword) || x.ProjectEn.ToLower().Contains(keyword)))
                                    .OrderByDescending(x => x.ApproveDate)
                                    .Select(x => new {
                                        Id = x.Id,
                                        ProjectVi = x.ProjectVi,
                                        ProjectEn = x.ProjectEn
                                    }).ToListAsync();

                var projectsExisted = projects.Where(x => x.ProjectVi.ToLower().Contains(request.Keyword.ToLower().Trim()) ||
                                            x.ProjectEn.ToLower().Contains(request.Keyword.ToLower().Trim()))
                                        .Take(10)
                                        .ToList();

                if (projectsExisted.Count() > 0)
                {
                    foreach (var item in projectsExisted)
                    {
                        result.Add(new SuggestSearchProjectModel
                        {
                            ProjectId = item.Id,
                            TitleEn = item.ProjectEn,
                            TitleVi = item.ProjectVi
                        });
                    }
                }

                return result;
            }
            else
            {
                var result = new List<SuggestSearchProjectModel>();

                return result;
            }
        }
    }
}
