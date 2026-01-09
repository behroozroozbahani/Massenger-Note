using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerPrivateMessages.Queries.GetAllMessengerPrivateMessages
{
    public class GetAllMessengerPrivateMessagesQuery : IRequest<IList<MessengerDisplayAllPrivateMessagesDto>>
    {

    }

    public class GetAllMessengerPrivateMessagesQueryHandler : IRequestHandler<GetAllMessengerPrivateMessagesQuery, IList<MessengerDisplayAllPrivateMessagesDto>>
    {
        private readonly IMessengerPrivateMessageService _messengerPrivateMessageService;

        public GetAllMessengerPrivateMessagesQueryHandler(IMessengerPrivateMessageService messengerPrivateMessageService)
        {
            _messengerPrivateMessageService = messengerPrivateMessageService;
        }

        public async Task<IList<MessengerDisplayAllPrivateMessagesDto>> Handle(GetAllMessengerPrivateMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _messengerPrivateMessageService.GetAllPrivateMessagesAsync(cancellationToken);
        }
    }
}