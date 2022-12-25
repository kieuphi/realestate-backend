using AutoMapper;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.PropertyHeart.Queries
{
    public class IsPropertyHeartQuery : IRequest<bool>
    {
        public CreatePropertyHeartModel Model { set; get; }
    }

    public class IsPropertyHeartQueryHandler : IRequestHandler<IsPropertyHeartQuery, bool>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public IsPropertyHeartQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(IsPropertyHeartQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            var result = await _context.PropertyHeart
                .Where(X => X.UserId == model.UserId && X.PropertyId == model.PropertyId)
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return false;
            }

            return true;
        }
    }
}
