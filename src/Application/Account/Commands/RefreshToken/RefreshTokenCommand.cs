using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Token;
using PortalCore.Application.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Account.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Token>
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Token>
    {
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IApplicationDbContext _context;
        private readonly IAntiForgeryCookieService _antiForgery;

        public RefreshTokenCommandHandler(
            ITokenFactoryService tokenFactoryService,
            ITokenStoreService tokenStoreService,
            IApplicationDbContext context,
            IAntiForgeryCookieService antiForgery)
        {
            _tokenFactoryService = tokenFactoryService;
            _tokenStoreService = tokenStoreService;
            _context = context;
            _antiForgery = antiForgery;
        }

        public async Task<Token> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = await _tokenStoreService.FindTokenAsync(request.RefreshToken);
            if (token is null)
            {
                throw new UnauthorizedAccessException();
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(token.User);
            await _tokenStoreService.AddUserTokenAsync(
                token.User,
                result.RefreshTokenSerial,
                result.AccessToken,
                _tokenFactoryService.GetRefreshTokenSerial(request.RefreshToken));
            await _context.SaveChangesAsync(cancellationToken);

            _antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            return new Token
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
            };
        }
    }
}
