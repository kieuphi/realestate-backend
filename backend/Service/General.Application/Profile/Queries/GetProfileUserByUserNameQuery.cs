using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Domain.Common;
using General.Domain.Models;

namespace General.Application.Profile.Queries
{
    public class GetProfileUserByUserNameQuery : IRequest<UserResult>
    {
        public string UserName { get; set; }
    }

    public class GetProfileUserByUserNameQueryHandler : IRequestHandler<GetProfileUserByUserNameQuery, UserResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetProfileUserByUserNameQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetProfileUserByUserNameQueryHandler(
            IMapper mapper,
            ILogger<GetProfileUserByUserNameQueryHandler> logger,
            IIdentityService identityService,
            IApplicationDbContext context)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserResult> Handle(GetProfileUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserName))
            {
                return null;
            }

            var user = _mapper.Map<UserResult>(await _identityService.GetUserByUserNameAsync(request.UserName));
            user.RoleName = await _identityService.GetRoleUserAsync(user.UserName);

            var resourceEntity = _context.ProfileInformation.Where(x => x.UserId == Guid.Parse(user.Id)).FirstOrDefault();
            user.PersonalInformation = _mapper.Map<ProfileInformationModel>(resourceEntity);

            return user;
        }
    }
}
