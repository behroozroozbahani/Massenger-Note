using PortalCore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerPrivateMessageAdminService
    {
        Task<IList<MessengerDisplayAllPrivateMessagesAdminDto>> GetAllPrivateMessagesAdminAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>> GetPrivateMessageBySenderRecipientIdAdminAsync(Guid senderRecipientId, CancellationToken cancellationToken);
    }
}