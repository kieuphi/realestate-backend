using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Supplier.Commands;
using General.Application.Supplier.Queries;

namespace General.Api.Controllers
{
    public class SupplierController : ApiController
    {
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ILogger<SupplierController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetById/{userId}")]
        [ProducesResponseType(typeof(ProfileInformationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProfileInformationModel>> GetById(Guid userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetSupplierByIdQuery() { UserId = userId });

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(UpdateProfileInformationModel model, Guid profileId)
        {
            if (profileId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateSupplierCommand()
            {
                Model = model,
                ProfileId = profileId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingSupplierModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProfileInformationModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProfileInformationModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProfileInformationModel>>> PagingSupplier(PagingSupplierModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingSupplierQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }
    }
}
