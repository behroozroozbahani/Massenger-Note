using PortalCore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerPrivateMessageService
    {
        Task<IList<MessengerDisplayAllPrivateMessagesDto>> GetAllPrivateMessagesAsync(CancellationToken cancellationToken);
        Task<MessengerDisplayMessagesDto> GetPrivateMessageBySenderRecipientIdAsync(Guid senderRecipientId, CancellationToken cancellationToken);
    }
}
