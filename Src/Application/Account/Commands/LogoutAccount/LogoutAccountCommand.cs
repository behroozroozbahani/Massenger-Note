using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Token;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.Account.Commands.LogoutAccount
{
    public class LogoutAccountCommand: IRequest
    {
        public LogoutAccountCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; set; }
    }

    public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand>
    {
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IApplicationDbContext _context;
        private readonly IAntiForgeryCookieService _antiForgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutAccountCommandHandler(ITokenStoreService tokenStoreService, IApplicationDbContext context, IAntiForgeryCookieService antiForgery, IHttpContextAccessor httpContextAccessor)
        {
            _tokenStoreService = tokenStoreService;
            _context = context;
            _antiForgery = antiForgery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _httpContextAccessor.HttpContext.GetUserId();

            // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
            // Delete the user's tokens from the database (revoke its bearer token)
            await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue.ToString(), request.RefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            _antiForgery.DeleteAntiForgeryCookies();

            return Unit.Value;
        }
    }
}
