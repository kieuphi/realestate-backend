using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Common.Shared.Models;
using System.Threading.Tasks;
using General.Application.Contact.Commands;
using General.Application.Contact.Queries;
using System.Collections.Generic;
using General.Application.Config.Commands;

namespace General.Api.Controllers
{
    public class ConfigController : ApiController
    {
        private readonly ILogger<ConfigController> _logger;

        public ConfigController(ILogger<ConfigController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Save")]
        [ProducesResponseType(typeof(CreateConfigModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> SaveConfig(CreateConfigModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new SaveConfigCommand() { Model = model });

            return Ok(result);
        }

        [HttpGet("GetConfig")]
        [ProducesResponseType(typeof(ConfigModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConfigModel>> GetConfig()
        {
            var result = await Mediator.Send(new GetConfigQuery());

            return Ok(result);
        }
    }
}
