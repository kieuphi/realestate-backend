using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.PropertyFavorite.Commands;
using General.Application.PropertyFavorite.Queries;
using Microsoft.AspNetCore.Authorization;

namespace General.Api.Controllers
{
    public class PropertyFavoriteController : ApiController
    {
        private readonly ILogger<PropertyFavoriteController> _logger;

        public PropertyFavoriteController(ILogger<PropertyFavoriteController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreatePropertyFavoriteModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreatePropertyFavoriteModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreatePropertyFavoriteCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingPropertyFavoriteModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> PagingProperty(PagingPropertyFavoriteModel model)
        {
            var result = await Mediator.Send(new GetPagingPropertyFavoriteByUserIdQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Delete(Guid propertyFavoriteId)
        {
            if (propertyFavoriteId == null) return BadRequest();

            var result = await Mediator.Send(new DeletePropertyFavoriteCommand() { PropertyFavoriteId = propertyFavoriteId });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}
