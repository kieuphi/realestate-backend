using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using Microsoft.AspNetCore.Http;
using General.Domain.Models;
using General.Application.Property.Commands;
using Microsoft.Extensions.Logging;
using General.Application.Property.Queries;
using General.Domain.Common.Excel;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using General.Domain.Enums;
using Common.Shared.Services;

namespace General.Api.Controllers
{
    public class PropertyController : ApiController
    {
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(ILogger<PropertyController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreatePropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreatePropertyModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreatePropertyCommand() { Model = model, IsSaveTemp = false });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("SaveTemp")]
        [ProducesResponseType(typeof(CreatePropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> SaveTemp(CreatePropertyModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreatePropertyCommand() { Model = model, IsSaveTemp = true });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListPropertyModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllPropertyQuery());
        }

        [Authorize]
        [HttpPost("FilterPropertyByUser")]
        [ProducesResponseType(typeof(List<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListPropertyModel>>> FilterPropertyByUser(FilterPropertyByUserModel filterData)
        {
            return await Mediator.Send(new FilterPropertyByUserQuery { FilterData = filterData });
        }


        [HttpGet("GetAllBasicInfo")]
        [ProducesResponseType(typeof(List<BasicPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BasicPropertyModel>>> GetAllBasicInfo()
        {
            return await Mediator.Send(new GetAllBasicInfoQuery());
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetPropertyByIdQuery() { Id = id, IsAdmin = false });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpGet("GetByIdAdmin")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyModel>> GetByIdAdmin([FromQuery] Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetPropertyByIdQuery() { Id = id, IsAdmin = true });

            return Ok(result);
        }

        [HttpGet("GetBySlug/{slug}")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyModel>> GetBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetPropertyBySlugQuery() { Slug = slug, IsAdmin = false });

            return Ok(result);
        }

        [HttpPost("GetByListId")]
        [ProducesResponseType(typeof(PropertyModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyModel>> GetByListId([FromBody] List<Guid> listId)
        {
            var result = await Mediator.Send(new GetByListIdQuery() { ListId = listId });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(UpdatePropertyModel model, Guid propertyId )
        {
            if (propertyId == null) return BadRequest();
            if (model == null) return BadRequest();

            Result result = await Mediator.Send(new UpdatePropertyCommand()
            {
                Model = model,
                PropertyId = propertyId,
            }); ;

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UpdatePropertyByUser")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UpdatePropertyByUser(CreatePropertyModel model, Guid propertyId)
        {
            if (propertyId == null || propertyId == Guid.Empty)
            {
                return BadRequest();
            }
            if (model == null)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new UpdatePropertyByUserCommand()
            {
                Model = model,
                PropertyId = propertyId,
            }); ;

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("ViewProperty")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> ViewProperty(Guid propertyId)
        {
            if (propertyId == null || propertyId == Guid.Empty)
            {
                return BadRequest();
            }

            Result result = await Mediator.Send(new ViewPropertyCommand()
            {
                PropertyId = propertyId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Approve")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Approve(ApprovePropertyModel model)
        {
            if (model.PropertyId == null) return BadRequest();

            Result result = await Mediator.Send(new ApprovePropertyCommand()
            {
                Model = model
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid propertyId)
        {
            if (propertyId == null) return BadRequest();

            Result result = await Mediator.Send(new DeletePropertyCommand()
            {
                PropertyId = propertyId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("DeleteTempProperty")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> DeleteTempProperty(Guid propertyId)
        {
            if (propertyId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteTempPropertyCommand()
            {
                PropertyId = propertyId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("Search")]
        [ProducesResponseType(typeof(SearchingPropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> SearchAndSortProperty(SearchingPropertyModel searchModel)
        {
            var result = await Mediator.Send(new SearchPropertyQuery() { 
                SearchModel = searchModel
            });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("SearchForAdmin")]
        [ProducesResponseType(typeof(SearchingPropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> SearchAndSortPropertyForAdmin(SearchingPropertyForAdminModel searchModel)
        {
            var result = await Mediator.Send(new SearchPropertyForAdminQuery()
            {
                SearchModel = searchModel
            });

            return Ok(result);
        }

        [HttpPost("SuggestProperty")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SuggestSearchPropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SuggestSearchPropertyModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<SuggestSearchPropertyModel>> SuggestProperty(SuggestSearchPropertyRequestModel model)
        {
            var result = await Mediator.Send(new SuggestPropertyQuery()
            {
                Keyword = model.Keyword,
                SuggestPropertyType = model.SuggestPropertyType
            });

            return Ok(result);
        }

        [HttpGet("GetTotalCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<int> GetTotalCount()
        {
            return await Mediator.Send(new GetPropertyCountQuery());
        }

        [HttpPost("GetNearest")]
        [ProducesResponseType(typeof(PagingNearestPropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> GetNearest(PagingNearestPropertyModel model)
        {
            var result = await Mediator.Send(new GetNearestPropertyQuery()
            {
                Model = model
            });

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingPropertyModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ListPropertyModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ListPropertyModel>>> PagingProperty(PagingPropertyModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingPropertyQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPut("UnPost")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnPost(Guid propertyId)
        {
            if (propertyId == null) return BadRequest();

            Result result = await Mediator.Send(new UnPostPropertyCommand()
            {
                PropertyId = propertyId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnLock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnLock(Guid propertyId)
        {
            if (propertyId == null) return BadRequest();

            Result result = await Mediator.Send(new UnLockPropertyCommand()
            {
                PropertyId = propertyId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("GenerateAllPropertySlug")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> GenerateAllPropertySlug()
        {
            Result result = await Mediator.Send(new GenerateMissingPropertySlugsCommand() { });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("CreateMany")]
        [ProducesResponseType(typeof(List<ImportPropertyResultModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CreateMany(List<ImportPropertyResultModel> model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreatePropertiesCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("Import")]
        [ProducesResponseType(typeof(List<Result>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Result>>> Import(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var data = ExcelReader.ReadExcelWorksheet(ms, true, true);

                var importModel = ConvertDataBeforeInsert(data);

                var result = await Mediator.Send(new ImportPropertyCommand { ImportModel = importModel });

                return Ok(result);
            }
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpGet("ExportMeetingNotes")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<FileResult> ExportMeetingNotes(Guid propertyId)
        {
            var vm = await Mediator.Send(new ExportMeetingNotesCommand { PropertyId = propertyId });
            var result = File(vm.Content, vm.ContentType, vm.FileName);
            return result;
        }


        private List<ImportPropertyModel> ConvertDataBeforeInsert(List<List<string>> dataSource)
        {
            var result = new List<ImportPropertyModel>();

            foreach (var item in dataSource)
            {
                var importModel = new ImportPropertyModel();

                var properties = typeof(ImportPropertyModel).GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyInfo = properties[i];

                    if (string.IsNullOrWhiteSpace(item[i]))
                    {
                        propertyInfo.SetValue(importModel, item[i]);
                        continue;
                    }

                    propertyInfo.SetValue(importModel, item[i].Replace(",", "."));
                }

                result.Add(importModel);
            }

            return result;
        }
    }
}
