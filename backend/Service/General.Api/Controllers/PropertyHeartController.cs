using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.PropertyHeart.Commands;
using General.Application.PropertyHeart.Queries;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace General.Api.Controllers
{
    public class PropertyHeartController : ApiController
    {
        private readonly ILogger<PropertyHeartController> _logger;

        public PropertyHeartController(ILogger<PropertyHeartController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("SendHeart")]
        [ProducesResponseType(typeof(CreatePropertyHeartModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> SendHeart(CreatePropertyHeartModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new SendPropertyHeartCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingPropertyHeartModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyHeartModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyHeartModel>>> PagingProperty(PagingPropertyHeartModel model)
        {
            var result = await Mediator.Send(new GetPagingPropertyHeartByUserIdQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Delete(Guid propertyHeartId)
        {
            if (propertyHeartId == null) return BadRequest();

            var result = await Mediator.Send(new DeletePropertyHeartCommand() { PropertyHeartId = propertyHeartId });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("IsPropertyHeart")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> IsPropertyHeart(CreatePropertyHeartModel model)
        {
            var result = await Mediator.Send(new IsPropertyHeartQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPut("IsManyPropertyHeart/{userId}")]
        [ProducesResponseType(typeof(List<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<bool>>> IsManyPropertyHeart(Guid userId, List<Guid> propertyIds)
        {
            if (propertyIds == null) return BadRequest();
            if (userId == null) return BadRequest();

            var result = await Mediator.Send(new IsManyPropertyHeartQuery()
            {
                UserId = userId,
                PropertyIds = propertyIds,
            });

            return Ok(result);
        }
    }
}
