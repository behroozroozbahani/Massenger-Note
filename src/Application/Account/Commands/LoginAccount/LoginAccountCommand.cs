using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Identity;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Common.Token;
using PortalCore.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Account.Commands.LoginAccount
{
    public class LoginAccountCommand : IRequest<Token>
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class LoginAcountCommandHandler : IRequestHandler<LoginAccountCommand, Token>
    {
        private readonly IApplicationDbContext _context;
        private readonly IApplicationUserManager _userManager;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IAntiForgeryCookieService _antiForgery;
        private readonly ITokenFactoryService _tokenFactoryService;

        public LoginAcountCommandHandler(IAntiForgeryCookieService antiForgery, IApplicationDbContext context, ITokenStoreService tokenStoreService, ITokenFactoryService tokenFactoryService, IApplicationUserManager userManager)
        {
            _context = context;
            _antiForgery = antiForgery;
            _userManager = userManager;
            _tokenStoreService = tokenStoreService;
            _tokenFactoryService = tokenFactoryService;
        }

        public async Task<Token> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                throw new BadRequestException("نام کاربری و یا کلمه‌ی عبور وارد شده معتبر نیستند.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new BadRequestException("نام کاربری و یا کلمه‌ی عبور وارد شده معتبر نیستند.");

            var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
            await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
            await _context.SaveChangesAsync(cancellationToken);

            _antiForgery.RegenerateAntiForgeryCookies(result.Claims);

            return new Token
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            };
        }
    }
}
