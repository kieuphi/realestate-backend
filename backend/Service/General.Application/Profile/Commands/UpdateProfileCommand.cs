using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Enums = Common.Shared.Enums;
using CommunicationMicroservice = Common.Shared.Microservice.CommunicationClient;
using AutoMapper;
using Microsoft.Extensions.Logging;
using General.Domain.Models;
using Common.Shared.Models;
using Common.Shared.Services;

namespace General.Application.Profile.Commands
{
    public class UpdateProfileCommand : IRequest<Result>
    {
        public UpdateProfileModel PersonalInformation { get;set;}
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly General.Application.Interfaces.ICurrentUserService _userService;

        public UpdateProfileCommandHandler(ILogger<UpdateProfileCommandHandler> logger,IIdentityService identityService,
            IMapper mapper, General.Application.Interfaces.ICurrentUserService userService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdentifierAsync(_userService.UserId);
            if (user == null)
            {
                return (Result.Failure("User has been NOT existed in system"));
            }

            var result =  await _identityService.UpdateProfileAsync(request.PersonalInformation, Guid.Parse(_userService.UserId));
            return result;
        }
    }
}
