using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.MessengerPrivateMessages.Queries.GetAllMessengerPrivateMessagesAdmin
{
    public class GetAllMessengerPrivateMessagesAdminQuery : IRequest<IList<MessengerDisplayAllPrivateMessagesAdminDto>>
    {

    }

    public class GetAllMessengerPrivateMessagesAdminQueryHandler : IRequestHandler<GetAllMessengerPrivateMessagesAdminQuery, IList<MessengerDisplayAllPrivateMessagesAdminDto>>
    {
        private readonly IMessengerPrivateMessageAdminService _messengerPrivateMessageAdminService;

        public GetAllMessengerPrivateMessagesAdminQueryHandler(IMessengerPrivateMessageAdminService messengerPrivateMessageAdminService)
        {
            _messengerPrivateMessageAdminService = messengerPrivateMessageAdminService;
        }

        public async Task<IList<MessengerDisplayAllPrivateMessagesAdminDto>> Handle(GetAllMessengerPrivateMessagesAdminQuery request, CancellationToken cancellationToken)
        {
            return await _messengerPrivateMessageAdminService.GetAllPrivateMessagesAdminAsync(cancellationToken);
        }
    }
}