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

namespace General.Application.ProjectViewCount.Commands
{
    public class CountViewProjectCommand : IRequest<Result>
    {
        public Guid ProjectId { set; get; }
    }

    public class CountViewProjectCommandHandler : IRequestHandler<CountViewProjectCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public CountViewProjectCommandHandler (
            IMapper mapper,
            IApplicationDbContext context
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(CountViewProjectCommand request, CancellationToken cancellationToken)
        {
            var projectId = request.ProjectId;
            var projectView = await _context.ProjectViewCount.Where(x => x.ProjectId == projectId).FirstOrDefaultAsync();

            if (projectView == null)
            {
                var newId = Guid.NewGuid();

                _context.ProjectViewCount.Add(new ProjectViewCountEntity
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    ViewCount = 1
                });
            }
            else
            {
                projectView.ViewCount = projectView.ViewCount + 1;
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
