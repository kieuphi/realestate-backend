using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Common.Shared.Models;
using System.Threading.Tasks;
using General.Application.Banner.Command;
using General.Application.Banner.Query;
using System.Collections.Generic;
using General.Domain.Enums;

namespace General.Api.Controllers
{
    public class BannerController : ApiController
    {
        private readonly ILogger<BannerController> _logger;

        public BannerController(ILogger<BannerController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateBannerModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateBannerModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateBannerCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(BannerModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BannerModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetBannerByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<BannerModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BannerModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllBannerQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            if (id == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteBannerCommand()
            {
                Id = id
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingBannerModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<BannerModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<BannerModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<BannerModel>>> PagingBanner(PagingBannerModel model)
        {
            var result = await Mediator.Send(new GetPagingBannerQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(Guid bannerId, CreateBannerModel model)
        {
            if (bannerId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateBannerCommand()
            {
                BannerId = bannerId,
                Model = model
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetByType")]
        [ProducesResponseType(typeof(List<BannerModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BannerModel>>> GetByType(BannerTypes type)
        {
            if (type == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetBannerFromTypeQuery() { Type = type });

            return Ok(result);
        }
    }
}
