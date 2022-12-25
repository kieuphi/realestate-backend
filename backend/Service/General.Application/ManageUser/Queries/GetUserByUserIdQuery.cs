using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Domain.Models;

namespace General.Application.ManageUser.Queries
{
    public class GetUserByUserIdQuery : IRequest<UserModel>
    {
        public string UserId { get; set; }
    }

    public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, UserModel>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetUserByUserIdQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetUserByUserIdQueryHandler(
            IMapper mapper,
            ILogger<GetUserByUserIdQueryHandler> logger,
            IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserModel> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdentifierAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            var roles = await _identityService.GetRolesUserAsync(user.UserName);
            var result = UserModel.ToUserModel(user);
            result.Role = roles.FirstOrDefault();
            return result;
        }
    }
}
