using General.Application.PropertyMeetingNote.Queries;
using General.Domain.Models.PropertyElementModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace General.Api.Controllers
{
    public class PropertyMeetingNoteController : ApiController
    {
        private readonly ILogger<PropertyMeetingNoteController> _logger;

        public PropertyMeetingNoteController(ILogger<PropertyMeetingNoteController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetById/{propertyId}")]
        [ProducesResponseType(typeof(PropertyMeetingNoteModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyMeetingNoteModel>> GetById(Guid propertyId)
        {
            if (propertyId == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetPropertyMeetingNoteByPropertyIdQuery() { PropertyId = propertyId });

            return Ok(result);
        }
    }
}
