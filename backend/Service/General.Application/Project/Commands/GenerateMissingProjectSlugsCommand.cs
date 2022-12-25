using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Project.Commands
{
    public class GenerateMissingProjectSlugsCommand : IRequest<Result>
    {
    }

    public class GenerateMissingProjectSlugsCommandHandler : IRequestHandler<GenerateMissingProjectSlugsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GenerateMissingProjectSlugsCommandHandler(
            ICommonFunctionService commonFunctionService,
            IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(GenerateMissingProjectSlugsCommand request, CancellationToken cancellationToken)
        {
            var properties = await _context.Project.ToListAsync();
            int count = 0;

            foreach(var item in properties)
            {
                count = count + 1;
                if (string.IsNullOrEmpty(item.Slug))
                {
                    item.Slug = _commonFunctionService.GenerateFriendlyUrl(item.ProjectEn, count);
                }
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
