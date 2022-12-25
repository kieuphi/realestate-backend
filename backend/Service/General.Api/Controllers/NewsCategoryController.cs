using General.Application.NewsCategory.Queries;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using System.Threading.Tasks;
using General.Application.NewsCategory.Commands;

namespace General.Api.Controllers
{
    public class NewsCategoryController : ApiController
    {
        private readonly ILogger<NewsCategoryController> _logger;

        public NewsCategoryController(ILogger<NewsCategoryController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateNewsCategoryModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateNewsCategoryModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateNewsCategoryCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(NewsCategoryModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewsCategoryModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetNewsCategoryByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ListNewsCategoryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListNewsCategoryModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllNewsCategoryQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid newsCategoryId)
        {
            if (newsCategoryId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteNewsCategoryCommand()
            {
                NewsCategoryId = newsCategoryId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(CreateNewsCategoryModel model, Guid newsCategoryId)
        {
            if (newsCategoryId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateNewsCategoryCommand()
            {
                Model = model,
                NewsCategoryId = newsCategoryId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingNewsCategoryModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsCategoryModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsCategoryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsCategoryModel>>> PagingNews(PagingNewsCategoryModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingNewsCategoryQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPost("SearchingForAdmin")]
        [ProducesResponseType(typeof(SearchingNewsCategoryModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsCategoryModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsCategoryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsCategoryModel>>> SearchingForAdmin(SearchingNewsCategoryModel model)
        {
            var result = await Mediator.Send(new SearchingNewsCategoryForAdminQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPut("Post")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Post(Guid newsCategoryId)
        {
            if (newsCategoryId == null) return BadRequest();

            Result result = await Mediator.Send(new PostNewsCategoryCommand()
            {
                NewsCategoryId = newsCategoryId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnPost")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnPost(Guid newsCategoryId)
        {
            if (newsCategoryId == null) return BadRequest();

            Result result = await Mediator.Send(new UnPostNewsCategoryCommand()
            {
                NewsCategoryId = newsCategoryId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Active")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Active(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new ActiveNewsCategoryCommand()
            {
                Id = id
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
        
        [HttpPut("InActive")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> InActive(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new InActiveNewsCategoryCommand()
            {
                Id = id
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Lock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Lock(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new LockNewsCategoryCommand()
            {
                Id = id
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnLock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnLock(Guid newsCategoryId)
        {
            if (newsCategoryId == null) return BadRequest();

            Result result = await Mediator.Send(new UnLockNewsCategoryCommand()
            {
                NewsCategoryId = newsCategoryId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}
