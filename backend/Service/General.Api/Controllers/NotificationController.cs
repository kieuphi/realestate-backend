using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Domain.Models;
using General.Application.Notification.Commands;
using Microsoft.AspNetCore.Authorization;
using General.Application.Notification.Queries;
using General.Domain.Enums;

namespace General.Api.Controllers
{
    public class NotificationController : ApiController
    {
        /**
         use for admin
         */
        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpGet("GetPaging")]
        [ProducesResponseType(typeof(PaginatedList<NotificationModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<NotificationModel>>> GetPaging([FromQuery] int pageNumber, int pageSize)
        {
            var result = await Mediator.Send(new GetPagingQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetById")]
        [ProducesResponseType(typeof(NotificationModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        public async Task<ActionResult<NotificationModel>> GetById([FromQuery] Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetByIdQuery()
            {
                Id = id
            });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateNotificationModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CreateNotification(CreateNotificationModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateNotificationCommand() { CreateNotificationModel = model });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("Filter")]
        [ProducesResponseType(typeof(SortingNotificationModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<NotificationModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<NotificationModel>>> Filter(FilterNotificationModel filterNotificationModel)
        {
            var result = await Mediator.Send(new GetFilterNotificationQuery()
            {
                FilterNotificationModel = filterNotificationModel
            });

            return Ok(result);
        }
        
        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateNotificationModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UpdateNotification(Guid id, UpdateNotificationModel updateNotificationModel)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new UpdateNotificationCommand()
            {
                Id = id,
                UpdateNotificationModel = updateNotificationModel
            });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("PostNotification")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateNotificationModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> PostNotification(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new PostNotificationCommand()
            {
                Id = id
            });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("UnPostNotification")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UpdateNotificationModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnPostNotification(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new UnPostNotificationCommand()
            {
                Id = id
            });

            return Ok(result);
        }

        /**
         use for user
         */

        [Authorize]
        [HttpGet("GetPagingByUser")]
        [ProducesResponseType(typeof(PaginatedList<NotificationUserModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<NotificationUserModel>>> GetPagingByUser([FromQuery] int pageNumber, int pageSize)
        {
            var result = await Mediator.Send(new GetPagingByUserQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetCountNotificationByUser")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCountNotificationByUser([FromQuery] bool? IsSeen)
        {
            var result = await Mediator.Send(new GetCountNotificationByUserQuery()
            {
                IsSeen = IsSeen
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("RemoveNotificationByUser")]
        [ProducesResponseType(typeof(CreateNotificationModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> RemoveNotificationByUser([FromBody] Guid id)
        {
            var result = await Mediator.Send(new RemoveNotificationByUserCommand() { Id = id });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("RemoveAllNotificationByUser")]
        [ProducesResponseType(typeof(CreateNotificationModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> RemoveAllNotificationByUser()
        {
            var result = await Mediator.Send(new RemoveAllNotificationByUserCommand());

            return Ok(result);
        }

        [Authorize]
        [HttpPost("SeenNotificationByUser")]
        [ProducesResponseType(typeof(CreateNotificationModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> SeenNotificationByUser([FromBody] List<Guid> listId)
        {
            var result = await Mediator.Send(new SeenNotificationByUserCommand() { listId = listId });

            return Ok(result);
        }
    }

}
