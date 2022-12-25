using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace General.Application.News.Commands
{
    public class GenerateMissingNewsSlugsCommand : IRequest<Result>
    {
    }

    public class GenerateMissingNewsSlugsCommandHandler : IRequestHandler<GenerateMissingNewsSlugsCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GenerateMissingNewsSlugsCommandHandler(
            ICommonFunctionService commonFunctionService,
            IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(GenerateMissingNewsSlugsCommand request, CancellationToken cancellationToken)
        {
            var properties = await _context.News.ToListAsync();
            int count = 0;

            foreach(var item in properties)
            {
                count = count + 1;

                if (string.IsNullOrEmpty(item.Slug))
                {
                    item.Slug = _commonFunctionService.GenerateFriendlyUrl(item.TitleEn, count);
                }
            }

            await _context.SaveChangesAsync(new CancellationToken());

            return Result.Success();
        }
    }
}
