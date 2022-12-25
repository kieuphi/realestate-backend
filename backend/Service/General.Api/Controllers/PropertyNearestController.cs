using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.PropertyNearest.Commands;
using General.Application.PropertyNearest.Queries;

namespace General.Api.Controllers
{
    public class PropertyNearestController : ApiController
    {
        private readonly ILogger<PropertyNearestController> _logger;

        public PropertyNearestController(ILogger<PropertyNearestController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreatePropertyNearestModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreatePropertyNearestModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreatePropertyNearestCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingPropertyNearestModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> PagingProperty(PagingPropertyNearestModel model)
        {
            var result = await Mediator.Send(new GetPagingPropertyNearestByUserIdQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Delete(Guid propertyNearestId)
        {
            if (propertyNearestId == null) return BadRequest();

            var result = await Mediator.Send(new DeletePropertyNearestCommand() { PropertyNearestId = propertyNearestId });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}
