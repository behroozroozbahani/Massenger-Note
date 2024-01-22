using PortalCore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerGroupMessageAdminService
    {
        Task<IList<MessengerDisplayAllGroupMessagesAdminDto>> GetAllGroupMessagesAdminAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>> GetGroupMessageByGroupIdAdminAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
