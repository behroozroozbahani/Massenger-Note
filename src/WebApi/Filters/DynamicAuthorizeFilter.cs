using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Constants;
using PortalCore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortalCore.WebApi.Filters
{
    public class DynamicAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationDbContext _context;
        private readonly DbSet<UserRole> _userRoles;
        private readonly IMemoryCache _memoryCache;

        public DynamicAuthorizeFilter(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _context = context;
            _userRoles = _context.Set<UserRole>();
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .SingleOrDefault();
            if (attribute is not null)
            {
                return;
            }

            if (_httpContextAccessor.HttpContext is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            List<Claim> claims = new();
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                var jtiValue = jwtSecurityToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Jti).Value;
                var userIdValue = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                var identity = new ClaimsIdentity();
                identity.AddClaims(jwtSecurityToken.Claims);
                _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(identity);

                var cacheClaims = _memoryCache.Get(jtiValue);
                if (cacheClaims is null)
                {
                    var userId = Guid.Parse(userIdValue);
                    var clientRoles = await _userRoles
                                       .Include(p => p.Role)
                                       .Where(p => p.UserId == userId)
                                       .Select(p => new ClientRole
                                       {
                                           Role = p.Role.Name,
                                           ClientRoleClaims = p.Role.Claims
                                               .Select(q => new ClientRoleClaim
                                               {
                                                   ClaimType = q.ClaimType,
                                                   ClaimValue = q.ClaimValue,
                                               })
                                               .ToList(),
                                       })
                                       .AsNoTracking()
                                       .ToListAsync();

                    if (clientRoles != null)
                    {
                        foreach (var clientRole in clientRoles)
                        {
                            if (clientRole.ClientRoleClaims != null)
                            {
                                foreach (var claim in clientRole.ClientRoleClaims)
                                {
                                    if (claim.ClaimType == ConstantPolicies.DynamicPermissionClaimType)
                                    {
                                        if (!string.IsNullOrWhiteSpace(claim.ClaimValue))
                                        {
                                            claims.Add(new Claim(ConstantPolicies.DynamicPermissionClaimType,
                                                claim.ClaimValue,
                                                ClaimValueTypes.String));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!claims.Any())
                    {
                        context.Result = new ForbidResult();
                        return;
                    }

                    _memoryCache.Set(jtiValue, claims, new MemoryCacheEntryOptions { Size = 1 });
                }
                else
                {
                    claims = (List<Claim>)cacheClaims;
                }

                if (_httpContextAccessor.HttpContext.User.Identity is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var routeData = _httpContextAccessor.HttpContext.GetRouteData();

            var areaName = routeData?.Values["area"]?.ToString();
            var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;

            var controllerName = routeData?.Values["controller"]?.ToString();
            var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

            var actionName = routeData?.Values["action"]?.ToString();
            var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;

            var currentClaimValue = $"{area}:{controller}:{action}";
            if (claims.Any(claim =>
                    claim.Type == ConstantPolicies.DynamicPermissionClaimType &&
                    claim.Value == currentClaimValue))
            {
                return;
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
