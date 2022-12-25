using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using General.Application.ImageCategory.Queries;
using General.Domain.Models;

namespace General.Api.Controllers
{
    public class ImageCategoryController : ApiController
    {
        private readonly ILogger<ImageCategoryController> _logger;

        public ImageCategoryController(IMediator mediator,
          ILogger<ImageCategoryController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImageCategoryModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ImageCategoryModel>>> GetAllImageCategory()
        {
            var result = await Mediator.Send(new GetAllImageCategoryQuery());
            return Ok(result);
        }
    }
}
