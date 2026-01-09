using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PortalCore.Application.Common.Token
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context);
    }
}
