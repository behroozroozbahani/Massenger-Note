﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using PortalCore.Application.Common.Token;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace PortalCore.Persistence.Services.Token
{
    public class AntiForgeryCookieService : IAntiForgeryCookieService
    {
        private const string XsrfTokenKey = "XSRF-TOKEN";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAntiforgery _antiforgery;
        private readonly IOptions<AntiforgeryOptions> _antiforgeryOptions;

        public AntiForgeryCookieService(
          IHttpContextAccessor contextAccessor,
          IAntiforgery antiforgery,
          IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
            _antiforgeryOptions = antiforgeryOptions ?? throw new ArgumentNullException(nameof(antiforgeryOptions));
        }

        public void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims)
        {
            var httpContext = _contextAccessor.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
              key: XsrfTokenKey,
              value: tokens.RequestToken,
              options: new CookieOptions
              {
                  HttpOnly = false // Now JavaScript is able to read the cookie
              });
        }

        public void DeleteAntiForgeryCookies()
        {
            var cookies = _contextAccessor.HttpContext.Response.Cookies;
            cookies.Delete(_antiforgeryOptions.Value.Cookie.Name);
            cookies.Delete(XsrfTokenKey);
        }
    }
}
