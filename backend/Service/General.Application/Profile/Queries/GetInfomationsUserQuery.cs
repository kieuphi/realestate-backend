using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Common.Results;
using General.Domain.Common;
using General.Domain.Models;
using General.Application.Interfaces;

namespace General.Application.Profile.Queries
{
    public class GetInfomationsUserQuery : IRequest<UserResult>
    {
        public string UserId { get; set; }
    }

    public class GetInfomationsUserQueryHandler : IRequestHandler<GetInfomationsUserQuery, UserResult>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<GetInfomationsUserQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetInfomationsUserQueryHandler(
            IMapper mapper,
            ILogger<GetInfomationsUserQueryHandler> logger,
            IIdentityService identityService,
            IApplicationDbContext context,
            ICommonFunctionService commonFunctionService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<UserResult> Handle(GetInfomationsUserQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return null;
            }

            var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(request.UserId));
            user.RoleName = await _identityService.GetRoleUserAsync(user.UserName);

            var resourceEntity = _context.ProfileInformation.Where(x => x.UserId == Guid.Parse(user.Id)).FirstOrDefault();
            user.PersonalInformation = _mapper.Map<ProfileInformationModel>(resourceEntity);
            if (string.IsNullOrEmpty(user.PersonalInformation.Avatar) == false)
            {
                user.PersonalInformation.AvatarUrl = _commonFunctionService.ConvertImageUrl(user.PersonalInformation.Avatar);
            }

            return user;
        }
    }
}
