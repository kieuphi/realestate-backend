using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using General.Domain.Models;
using General.Application.AttachmentType.Queries;
using General.Application.AttachmentType.Commands;
using Common.Shared.Models;

namespace General.Api.Controllers
{
    public class AttachmentTypeController : ApiController
    {
        private readonly ILogger<AttachmentTypeController> _logger;

        public AttachmentTypeController(IMediator mediator,
          ILogger<AttachmentTypeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AttachmentTypeModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AttachmentTypeModel>>> GetAllAttachmentTypes()
        {
            var result = await Mediator.Send(new GetAllAttachmentTypesQuery());
            return Ok(result);
        }

        [HttpGet("Detail/{id}")]
        [ProducesResponseType(typeof(AttachmentTypeModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AttachmentTypeModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttachmentTypeModel>> GetAttachmentType(Guid id)
        {
            var result = await Mediator.Send(new GetAttachmentTypeByIdQuery() { Id = id });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //[HttpPut("{key}")]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(AttachmentTypeModel), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<Result>> UpdateAttachmentType(Guid key, [FromBody] AttachmentTypeModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(model);
        //    }

        //    var result = await _mediator.Send(new UpdateAttachmentTypeCommand() { Id = key, Entity = model });

        //    if (!result.Succeeded)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}

        //[HttpPost]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(AttachmentTypeModel), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<Result>> AddAttachmentType([FromBody] AttachmentTypeModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(model);
        //    }

        //    var result = await Mediator.Send(new AddAttachmentTypeCommand() { Model = model });

        //    if (!result.Succeeded)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<Result>> DeleteVideoHomePage(Guid id)
        //{
        //    var result = await _mediator.Send(new DeleteAttachmentTypeCommand() { Id = id });

        //    if (!result.Succeeded)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}
    }
}
