using System.Linq;
using PortalCore.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace PortalCore.Persistence.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description));
        }
    }
}