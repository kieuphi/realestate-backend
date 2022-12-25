using General.Application.Project.Queries;
using General.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using General.Application.Project.Commands;
using General.Application.ProjectViewCount.Commands;
using General.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using General.Domain.Common.Excel;

namespace General.Api.Controllers
{
    public class ProjectController : ApiController
    {
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(CreateProjectModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> Create(CreateProjectModel model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateProjectCommand() { Model = model });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        [ProducesResponseType(typeof(ProjectModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectModel>> GetById(Guid id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetProjectByIdQuery() { Id = id });

            return Ok(result);
        }

        [HttpGet("GetBySlug/{slug}")]
        [ProducesResponseType(typeof(ProjectModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectModel>> GetBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest();
            }

            var result = await Mediator.Send(new GetProjectBySlugQuery() { Slug = slug });

            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(List<ProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProjectModel>>> GetAll()
        {
            return await Mediator.Send(new GetAllProjectQuery());
        }

        [HttpPut("Delete")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Delete(Guid projectId)
        {
            if (projectId == null) return BadRequest();

            Result result = await Mediator.Send(new DeleteProjectCommand()
            {
                ProjectId = projectId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("Update")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Update(UpdateProjectModel model, Guid projectId)
        {
            if (projectId == null) return BadRequest();

            Result result = await Mediator.Send(new UpdateProjectCommand()
            {
                Model = model,
                ProjectId = projectId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("GetPaging")]
        [ProducesResponseType(typeof(PagingProjectModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProjectModel>>> PagingProject(PagingProjectModel pagingModel)
        {
            var result = await Mediator.Send(new GetPagingProjectQuery()
            {
                PagingModel = pagingModel
            });

            return Ok(result);
        }

        [HttpPost("Search")]
        [ProducesResponseType(typeof(SearchProjectModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProjectModel>>> Search(SearchProjectModel model)
        {
            var result = await Mediator.Send(new SearchProjectQuery() { Model = model });

            return Ok(result);
        }

        [HttpPut("CountViewProject")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CountViewProject(Guid projectId)
        {
            if (projectId == null) return BadRequest();
            var result = await Mediator.Send(new CountViewProjectCommand() { ProjectId = projectId });

            return Ok(result);
        }

        [HttpGet("GetAdministrativeByProject")]
        [ProducesResponseType(typeof(List<AdministrativeByProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AdministrativeByProjectModel>>> GetAdministrativeByProject()
        {
            return await Mediator.Send(new GetAdministrativeByProjectQuery());
        }

        [HttpPost("SuggestProject")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<SuggestSearchProjectModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<SuggestSearchProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SuggestSearchProjectModel>>> SuggestProject(string keyword)
        {
            var result = await Mediator.Send(new SuggestProjectQuery()
            {
                Keyword = keyword
            });

            return Ok(result);
        }

        [HttpPut("Post")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> Post(Guid projectId)
        {
            if (projectId == null) return BadRequest();

            Result result = await Mediator.Send(new PostProjectCommand()
            {
                ProjectId = projectId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnPost")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnPost(Guid projectId)
        {
            if (projectId == null) return BadRequest();

            Result result = await Mediator.Send(new UnPostProjectCommand()
            {
                ProjectId = projectId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("UnLock")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> UnLock(Guid projectId)
        {
            if (projectId == null) return BadRequest();

            Result result = await Mediator.Send(new UnLockProjectCommand()
            {
                ProjectId = projectId
            });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPut("GenerateAllProjectSlug")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> GenerateAllProjectSlug()
        {
            Result result = await Mediator.Send(new GenerateMissingProjectSlugsCommand() { });

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("SearchForAdmin")]
        [ProducesResponseType(typeof(SearchProjectModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PaginatedList<ProjectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProjectModel>>> SearchForAdmin(SearchProjectForAdminModel model)
        {
            var result = await Mediator.Send(new SearchProjectForAdminQuery() { Model = model });

            return Ok(result);
        }

        [Authorize(Roles = Roles.SystemAdministrator)]
        [HttpPost("CreateMany")]
        [ProducesResponseType(typeof(List<ImportProjectResultModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> CreateMany(List<ImportProjectResultModel> model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateProjectsCommand() { Model = model });

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

                var result = await Mediator.Send(new ImportProjectCommand { ImportModel = importModel });

                return Ok(result);
            }
        }

        private List<ImportProjectModel> ConvertDataBeforeInsert(List<List<string>> dataSource)
        {
            var result = new List<ImportProjectModel>();

            foreach (var item in dataSource)
            {
                var importModel = new ImportProjectModel();

                var properties = typeof(ImportProjectModel).GetProperties();

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
