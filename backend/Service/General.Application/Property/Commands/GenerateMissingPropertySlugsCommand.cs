using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Property.Commands
{
    public class GenerateMissingPropertySlugsCommand : IRequest<Result>
    {
    }

    public class GenerateMissingPropertySlugsCommandHandler : IRequestHandler<GenerateMissingPropertySlugsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GenerateMissingPropertySlugsCommandHandler(
            ICommonFunctionService commonFunctionService,
            IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(GenerateMissingPropertySlugsCommand request, CancellationToken cancellationToken)
        {
            var properties = await _context.Property.ToListAsync();
            int count = 0;

            foreach(var item in properties)
            {
                count = count + 1;
                if (string.IsNullOrEmpty(item.Slug))
                {
                    item.Slug = _commonFunctionService.GenerateFriendlyUrl(item.Title, count);
                }
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
