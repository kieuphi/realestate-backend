using General.Application.News.Queries;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.News.Commands;
using General.Application.NewsViewCount.Commands;
using General.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace General.Api.Controllers
{
    public class NewsController : ApiController
    {
        private readonly ILogger<NewsController> _logger;

        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateNewsModel model)
        {
            if (!ModelState.IsValid) return BadRequest(); 

            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateNewsCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(NewsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewsModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetNewsByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetBySlug/{slug}")]
        [ProducesResponseType(typeof(NewsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewsModel>> GetBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetNewsBySlugQuery() { Slug = slug });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListNewsModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllNewsQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid newsId)
        {
            if (newsId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteNewsCommand()
            {
                NewsId = newsId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(CreateNewsModel model, Guid newsId)
        {
            if (newsId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateNewsCommand()
            {
                Model = model,
                NewsId = newsId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsModel>>> PagingNews(PagingNewsModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingNewsQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPost("Searching")]
        [ProducesResponseType(typeof(SearchingNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsModel>>> Searching(SearchingNewsModel model)
        {
            var result = await Mediator.Send(new SearchingNewsQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPost("SearchingForAdmin")]
        [ProducesResponseType(typeof(SearchingNewsForAdminModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsModel>>> SearchingForAdmin(SearchingNewsForAdminModel model)
        {
            var result = await Mediator.Send(new SearchingNewsForAdminQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPut("Post")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Post(Guid newsId)
        {
            if (newsId == null) return BadRequest();

            Result result = await Mediator.Send(new PostNewsCommand()
            {
                NewsId = newsId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnPost")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnPost(Guid newsId)
        {
            if (newsId == null) return BadRequest();

            Result result = await Mediator.Send(new UnPostNewsCommand()
            {
                NewsId = newsId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnLock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnLock(Guid newsId)
        {
            if (newsId == null) return BadRequest();

            Result result = await Mediator.Send(new UnLockNewsCommand()
            {
                NewsId = newsId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetHotNews")]
        [ProducesResponseType(typeof(PagingNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsModel>>> GetHotNews(PagingNewsModel pagingModel)
        {
            var result = await Mediator.Send(new GetHotNewsQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPost("GeWellReadNews")]
        [ProducesResponseType(typeof(PagingNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListNewsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListNewsModel>>> GeWellReadNews(PagingNewsModel pagingModel)
        {
            var result = await Mediator.Send(new GetWellReadNewsQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpGet("GetNewsGroupByCategory")]
        [ProducesResponseType(typeof(List<ListNewsGroupByCategoryModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ListNewsGroupByCategoryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListNewsGroupByCategoryModel>>> GetNewsGroupByCategory()
        {
            var result = await Mediator.Send(new GetNewsGroupByCategoryQuery() {});

            return Ok(result);
        }

        [HttpPut("GetByCategoryId/{categoryId}")]
        [ProducesResponseType(typeof(PaginatedList<NewsModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedList<NewsModel>>> GetByCategoryId(Guid categoryId, PagingNewsModel pagingModel)
        {
            if (categoryId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetNewsByCategoryQuery() 
            { 
                CategoryId = categoryId,
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPut("CountViewNews")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CountViewNews(Guid newsId)
        {
            if (newsId == null) return BadRequest();
            var result = await Mediator.Send(new CountViewNewsCommand() { NewsId = newsId });

            return Ok(result);
        }

        [HttpGet("GetNextNews/{currentNewsId}")]
        [ProducesResponseType(typeof(ListNewsModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ListNewsModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ListNewsModel>> GetNextNews(Guid currentNewsId)
        {
            if (currentNewsId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetNextNewsQuery() { CurrentNewsId = currentNewsId });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("GenerateAllNewsSlug")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> GenerateAllNewsSlug()
        {
            Result result = await Mediator.Send(new GenerateMissingNewsSlugsCommand() { });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }
}
