using PortalCore.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace PortalCore.Application.Common.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult identityResult)
        {
            return identityResult.Succeeded
                ? Result.Success()
                : Result.Failure(identityResult.Errors.Select(e => e.Description));
        }

        public static IDictionary<string, string[]> GetErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors
                .GroupBy(e => e.Code, e => e.Description)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public static IList<string> GetDescriptionErrors(this IdentityResult identityResult)
        {
            return identityResult.Errors
                .Select(p => p.Description)
                .ToList();
        }
    }
}
