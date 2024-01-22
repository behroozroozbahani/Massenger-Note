using System.Threading.Tasks;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Common.Token
{
    public interface ITokenFactoryService
    {
        Task<JwtTokensData> CreateJwtTokensAsync(User user);
        Task<JwtTokensData> CreateJwtTokensAsync(JwtUserInfo user);
        string GetRefreshTokenSerial(string refreshTokenValue);
    }
}
