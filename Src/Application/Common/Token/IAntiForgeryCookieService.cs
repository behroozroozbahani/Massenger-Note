using System.Collections.Generic;
using System.Security.Claims;

namespace PortalCore.Application.Common.Token
{
    public interface IAntiForgeryCookieService
    {
        void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
        void DeleteAntiForgeryCookies();
    }
}
