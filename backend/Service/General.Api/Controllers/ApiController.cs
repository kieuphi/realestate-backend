using Common.Shared.Localize;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace General.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
        private IMediator _mediator;
        private ILocalizationParser _localizationParser;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ILocalizationParser LocalizationParser => _localizationParser ??= HttpContext.RequestServices.GetService<ILocalizationParser>();

        protected string GetDomain() => $"{HttpContext.Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.ToUriComponent()}";
    }
}
