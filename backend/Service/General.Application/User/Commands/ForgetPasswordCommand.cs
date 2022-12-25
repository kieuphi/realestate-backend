using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Application.Common.Results;
using Common.Shared.Models;
using Microservice = Common.Shared.Microservice;
using General.Domain.Enums;
using System.Linq;
using Common.Shared.Services;
using General.Application.Email.Commands;
using General.Domain.Models;

namespace General.Application.User.Commands
{
    public class ForgetPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public ForgetPasswordCommandHandler(
            IIdentityService identityService,
            IMediator mediator
        ) {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure("userDoesNotExist");
            }
            string token = await _identityService.GeneratePasswordResetTokenAsync(user);
            //USING EMAIL
            var data = new ForgotPasswordModel()
            {
                Email = user.Email,
                Token = token,
                UserRole = Roles.InternalUser
            };
            try
            {
                var result = await _mediator.Send(new SendForgotPasswordEmailCommand() { Model = data });
                if (result.Succeeded == false)
                {
                    return Result.Failure(result.Errors.ToList());
                }
            }
            catch(Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }
    }
}
