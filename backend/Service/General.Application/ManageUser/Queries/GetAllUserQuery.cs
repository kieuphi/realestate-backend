using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;

namespace General.Application.ManageUser.Queries
{
    public class GetAllUserQuery : IRequest<List<UserResult>>
    {
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, List<UserResult>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetAllUserQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllUserQueryHandler(
            IMapper mapper,
            ILogger<GetAllUserQueryHandler> logger,
            IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<UserResult>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var users = _mapper.Map<List<UserResult>>(await _identityService.GetUsersAsync());

            users.ForEach(i =>
            {
                i.RoleName = _identityService.GetRoleUserAsync(i.UserName).Result;
            });

            return users;
        }
    }
}
