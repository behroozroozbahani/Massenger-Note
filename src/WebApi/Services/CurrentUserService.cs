using System;
using System.Security.Claims;
using PortalCore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PortalCore.WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                Guid? userId = null;
                var userIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdValue, out Guid result))
                {
                    userId = result;
                }

                return userId;
            }
        }
    }
}
