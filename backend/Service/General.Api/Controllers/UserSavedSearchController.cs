using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Domain.Models;
using General.Application.UserSavedSearch.Commands;
using Microsoft.AspNetCore.Authorization;
using General.Application.UserSavedSearch.Queries;

namespace General.Api.Controllers
{
    public class UserSavedSearchController : ApiController
    {
        [Authorize]
        [HttpPost("CreateSavedSearch")]
        [ProducesResponseType(typeof(CreateUserSavedSearchModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CreateSavedSearch(CreateUserSavedSearchModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateSavedSearchCommand() { CreateUserSavedSearchModel = model });

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Sorting")]
        [ProducesResponseType(typeof(SortingUserSavedSearchModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<UserSavedSearchModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<UserSavedSearchModel>>> Sorting(SortingUserSavedSearchModel sortingUserSavedSearchModel)
        {
            var result = await Mediator.Send(new GetSortingUserSavedSearchQuery()
            {
                SortingUserSavedSearchModel = sortingUserSavedSearchModel
            });

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {

            var result = await Mediator.Send(new DeleteUserSavedSearchCommand() { Id = id });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("DeleteAll")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> DeleteAll()
        {
            var result = await Mediator.Send(new DeleteAllUserSavedSearchCommand() { });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }
    }

}
