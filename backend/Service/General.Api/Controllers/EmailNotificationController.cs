using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Domain.Models;
using General.Application.Email.Commands;

namespace General.Api.Controllers
{
    public class EmailNotificationController : ApiController
    {
        private readonly ILogger<EmailNotificationController> _logger;

        public EmailNotificationController(ILogger<EmailNotificationController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //[HttpPost("verify-account")]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<Result>> VerifyAccount([FromBody] ConfirmationAccountModel request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(request);
        //    }

        //    return Ok(await Mediator.Send(new SendVerifyEmailCommand
        //    {
        //        RequestModel = request
        //    }));
        //}

        //[HttpPost("forgot-password")]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<Result>> ForgotPassword([FromBody] ForgotPasswordModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(model);
        //    }

        //    return Ok(await Mediator.Send(new SendForgotPasswordEmailCommand
        //    {
        //        Model = model
        //    }));
        //}
    }
}
