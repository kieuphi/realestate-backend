using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Common.Shared.Models;
using System.Threading.Tasks;
using General.Application.Contact.Commands;
using General.Application.Contact.Queries;
using System.Collections.Generic;

namespace General.Api.Controllers
{
    public class ContactController : ApiController
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(ContactModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateContactModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateContactCommand() { Model = model, Domain = GetDomain() });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(ContactModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetContactByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ContactModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ContactModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllContactQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid contactId)
        {
            if (contactId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteContactCommand()
            {
                ContactId = contactId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(CreateContactModel model, Guid contactId)
        {
            if (contactId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateContactCommand()
            {
                Model = model,
                ContactId = contactId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingContactModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ContactModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ContactModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ContactModel>>> PagingContact(PagingContactModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingContactQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }
    }
}
