using AutoMapper;
using AutoMapper.QueryableExtensions;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Contact.Queries
{
    public class GetConfigQuery : IRequest<ConfigModel>
    {
    }

    public class GetConfigQueryHandler : IRequestHandler<GetConfigQuery, ConfigModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetConfigQueryHandler (
            IMapper mapper,
            IApplicationDbContext context
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ConfigModel> Handle(GetConfigQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Config
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
            var data = _mapper.Map<ConfigModel>(entity);
            if (entity != null)
            {
                data.ListReceiveEmailContactUs = string.IsNullOrEmpty(entity.ReceiveEmailContactUs) == false ? entity.ReceiveEmailContactUs.Split(";").ToList() : new List<string>();
                data.ListReceiveEmailBookShowing = string.IsNullOrEmpty(entity.ReceiveEmailBookShowing) == false ? entity.ReceiveEmailBookShowing.Split(";").ToList() : new List<string>();
            }

            return data;
        }
    }
}
