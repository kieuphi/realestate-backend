using General.Application.SocialNetwork.Queries;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.SocialNetwork.Commands;

namespace General.Api.Controllers
{
    public class SocialNetworkController : ApiController
    {
        private readonly ILogger<SocialNetworkController> _logger;

        public SocialNetworkController(ILogger<SocialNetworkController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateSocialNetworkModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateSocialNetworkModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateSocialNetworkCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(SocialNetworkModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SocialNetworkModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetSocialNetworkByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid socialNetworkId)
        {
            if (socialNetworkId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteSocialNetworkCommand()
            {
                SocialNetworkId = socialNetworkId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(CreateSocialNetworkModel model, Guid socialNetworkId)
        {
            if (socialNetworkId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateSocialNetworkCommand()
            {
                Model = model,
                SocialNetworkId = socialNetworkId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingSocialNetworkModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<SocialNetworkModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<SocialNetworkModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<SocialNetworkModel>>> PagingSocialNetwork(PagingSocialNetworkModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingSocialNetworkQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }
    }
}
