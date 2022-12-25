using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Notification.Queries
{
    public class GetByIdQuery : IRequest<NotificationModel>
    {
        public Guid Id { get; set; }
    }

    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, NotificationModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetByIdQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<NotificationModel> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Notification
                .Where(x => x.Id == request.Id)
                .ProjectTo<NotificationModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return data;
        }
    }
}
