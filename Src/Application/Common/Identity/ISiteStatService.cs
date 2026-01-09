using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Common.Identity
{
    public interface ISiteStatService
    {
        Task<List<User>> GetOnlineUsersListAsync(int numbersToTake, int minutesToTake);

        Task UpdateUserLastVisitDateTimeAsync(ClaimsPrincipal claimsPrincipal);
    }
}