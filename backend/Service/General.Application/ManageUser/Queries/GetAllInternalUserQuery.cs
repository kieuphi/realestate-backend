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
using General.Domain.Enums;
using General.Domain.Models;

namespace General.Application.ManageUser.Queries
{
    public class GetAllInternalUserQuery : IRequest<List<UserModel>>
    {
    }

    public class GetAllInternalUserQueryHandler : IRequestHandler<GetAllInternalUserQuery, List<UserModel>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetAllInternalUserQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllInternalUserQueryHandler(
            IMapper mapper,
            ILogger<GetAllInternalUserQueryHandler> logger,
            IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<UserModel>> Handle(GetAllInternalUserQuery request, CancellationToken cancellationToken)
        {
            var listApplicationUser = await _identityService.GetUsersInRoleAsync(Roles.InternalUser);
            List<UserModel> listUser = new List<UserModel>();
            foreach (var item in listApplicationUser)
            {
                UserModel user = UserModel.ToUserModel(item);
                listUser.Add(user);
            }

            return listUser;
        }
    }
}
