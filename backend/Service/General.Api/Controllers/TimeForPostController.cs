using General.Application.TimeForPost.Queries;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.TimeForPost.Commands;

namespace General.Api.Controllers
{
    public class TimeForPostController : ApiController
    {
        private readonly ILogger<TimeForPostController> _logger;

        public TimeForPostController(ILogger<TimeForPostController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateTimeForPostModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateTimeForPostModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateTimeForPostCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(TimeForPostModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TimeForPostModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetTimeForPostByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<TimeForPostModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TimeForPostModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllTimeForPostQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid timeForPostId)
        {
            if (timeForPostId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteTimeForPostCommand()
            {
                TimeForPostId = timeForPostId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(CreateTimeForPostModel model, Guid timeForPostId)
        {
            if (timeForPostId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateTimeForPostCommand()
            {
                Model = model,
                TimeForPostId = timeForPostId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingTimeForPostModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<TimeForPostModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<TimeForPostModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<TimeForPostModel>>> PagingTimeForPost(PagingTimeForPostModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingTimeForPostQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }
    }
}
