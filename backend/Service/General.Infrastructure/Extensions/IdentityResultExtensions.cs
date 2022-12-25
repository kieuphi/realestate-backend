using Microsoft.AspNetCore.Identity;
using General.Application.Common.Results;
using Common.Shared.Models;
using System.Linq;

namespace General.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this Microsoft.AspNetCore.Identity.IdentityResult result)
        {
            var resultMessage = result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join("<br/>", result.Errors.Select(x => x.Code)));
            return resultMessage;
        }
    }
}
