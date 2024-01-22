using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerGroupMessages.Queries.GetAllMessengerGroupMessages
{
    public class GetAllMessengerGroupMessagesQuery : IRequest<IList<MessengerDisplayAllGroupMessagesDto>>
    {

    }

    public class GetAllMessengerGroupMessagesQueryHandler : IRequestHandler<GetAllMessengerGroupMessagesQuery, IList<MessengerDisplayAllGroupMessagesDto>>
    {
        private readonly IMessengerGroupMessageService _messengerGroupMessageService;

        public GetAllMessengerGroupMessagesQueryHandler(IMessengerGroupMessageService messengerGroupMessageService)
        {
            _messengerGroupMessageService = messengerGroupMessageService;
        }

        public async Task<IList<MessengerDisplayAllGroupMessagesDto>> Handle(GetAllMessengerGroupMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _messengerGroupMessageService.GetAllGroupMessagesAsync(cancellationToken);
        }
    }
}