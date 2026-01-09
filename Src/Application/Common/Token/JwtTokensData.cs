using System.Collections.Generic;
using System.Security.Claims;

namespace PortalCore.Application.Common.Token
{
    public class JwtTokensData
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string RefreshTokenSerial { get; set; } = null!;
        public IEnumerable<Claim> Claims { get; set; } = null!;
    }
}
