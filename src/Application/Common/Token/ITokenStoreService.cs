using System;
using System.Threading.Tasks;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Common.Token
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(JwtUserToken userToken);
        Task AddUserTokenAndSaveChangesAsync(JwtUserToken userToken);
        Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task AddUserTokenAsync(Guid userId, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task<bool> IsValidTokenAsync(string accessToken, Guid userId);
        Task DeleteExpiredTokensAsync();
        Task<JwtUserToken?> FindTokenAsync(string refreshTokenValue);
        Task DeleteTokenAsync(string refreshTokenValue);
        Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        Task InvalidateUserTokensAsync(Guid userId);
        Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue);
        Task RevokeUserBearerTokensAndSaveChangesAsync(string userIdValue, string refreshTokenValue);
    }
}
