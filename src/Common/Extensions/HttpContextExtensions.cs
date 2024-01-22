using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace PortalCore.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext? httpContext)
        {
            var claimsIdentity = httpContext?.User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value ?? string.Empty;
            if (Guid.TryParse(userIdValue, out Guid userId))
            {
                if (userId.IsNullOrEmpty())
                {
                    throw new UnauthorizedAccessException();
                }

                return userId;
            }

            throw new UnauthorizedAccessException();
        }
    }
}
