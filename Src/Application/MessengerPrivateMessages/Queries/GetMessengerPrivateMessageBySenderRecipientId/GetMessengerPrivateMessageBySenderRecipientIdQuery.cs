using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageBySenderRecipientId
{
    public class GetMessengerPrivateMessageBySenderRecipientIdQuery : IRequest<MessengerDisplayMessagesDto>
    {
        public Guid SenderRecipientId { get; set; }
    }

    public class GetPrivateMessageBySenderRecipientIdQueryHandler : IRequestHandler<GetMessengerPrivateMessageBySenderRecipientIdQuery, MessengerDisplayMessagesDto>
    {
        private readonly IMessengerPrivateMessageService _messengerPrivateMessageService;

        public GetPrivateMessageBySenderRecipientIdQueryHandler(IMessengerPrivateMessageService messengerPrivateMessageService)
        {
            _messengerPrivateMessageService = messengerPrivateMessageService;
        }

        public async Task<MessengerDisplayMessagesDto> Handle(GetMessengerPrivateMessageBySenderRecipientIdQuery request, CancellationToken cancellationToken)
        {
            return await _messengerPrivateMessageService.GetPrivateMessageBySenderRecipientIdAsync(request.SenderRecipientId, cancellationToken);
        }
    }
}