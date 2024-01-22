using PortalCore.Application.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerAllUsersService
    {
        Task<IList<MessengerDisplayAllUsersDto>> GetAllUsersAsync(CancellationToken cancellationToken);
    }
}
