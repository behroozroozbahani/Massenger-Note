using PortalCore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerGroupMessageService
    {
        Task<IList<MessengerDisplayAllGroupMessagesDto>> GetAllGroupMessagesAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>> GetGroupMessagesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    }
}
