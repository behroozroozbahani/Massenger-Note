using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageAdminBySenderRecipientId
{
    public class GetMessengerPrivateMessageAdminBySenderRecipientIdQuery : IRequest<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>>
    {
        public Guid SenderRecipientId { get; set; }
    }

    public class GetMessengerPrivateMessageAdminBySenderRecipientIdQueryHandler : IRequestHandler<GetMessengerPrivateMessageAdminBySenderRecipientIdQuery, IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>>
    {
        private readonly IMessengerPrivateMessageAdminService _messengerPrivateMessageAdminService;

        public GetMessengerPrivateMessageAdminBySenderRecipientIdQueryHandler(IMessengerPrivateMessageAdminService messengerPrivateMessageAdminService)
        {
            _messengerPrivateMessageAdminService = messengerPrivateMessageAdminService;
        }

        public async Task<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>> Handle(GetMessengerPrivateMessageAdminBySenderRecipientIdQuery request, CancellationToken cancellationToken)
        {
            return await _messengerPrivateMessageAdminService.GetPrivateMessageBySenderRecipientIdAdminAsync(request.SenderRecipientId, cancellationToken);
        }
    }
}