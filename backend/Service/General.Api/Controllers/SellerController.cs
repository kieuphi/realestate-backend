using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Seller.Commands;
using General.Application.Seller.Queries;

namespace General.Api.Controllers
{
    public class SellerController : ApiController
    {
        private readonly ILogger<SellerController> _logger;

        public SellerController(ILogger<SellerController> logger)
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

            var result = await Mediator.Send(new GetSellerByIdQuery() { UserId = userId });

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(UpdateProfileInformationModel model, Guid profileId)
        {
            if (profileId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateSellerCommand()
            {
                Model = model,
                ProfileId = profileId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingSellerModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProfileInformationModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProfileInformationModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProfileInformationModel>>> PagingSeller(PagingSellerModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingSellerQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }
    }
}
