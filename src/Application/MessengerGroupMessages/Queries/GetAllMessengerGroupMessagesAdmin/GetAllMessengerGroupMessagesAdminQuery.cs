using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerGroupMessages.Queries.GetAllMessengerGroupMessagesAdmin
{
    public class GetAllMessengerGroupMessagesAdminQuery : IRequest<IList<MessengerDisplayAllGroupMessagesAdminDto>>
    {
        public Guid GroupId { get; set; }
    }

    public class GetAllMessengerGroupMessagesAdminQueryHandler : IRequestHandler<GetAllMessengerGroupMessagesAdminQuery, IList<MessengerDisplayAllGroupMessagesAdminDto>>
    {
        private readonly IMessengerGroupMessageAdminService _messengerGroupMessageAdminService;

        public GetAllMessengerGroupMessagesAdminQueryHandler(IMessengerGroupMessageAdminService messengerGroupMessageAdminService)
        {
            _messengerGroupMessageAdminService = messengerGroupMessageAdminService;
        }

        public async Task<IList<MessengerDisplayAllGroupMessagesAdminDto>> Handle(GetAllMessengerGroupMessagesAdminQuery request, CancellationToken cancellationToken)
        {
            return await _messengerGroupMessageAdminService.GetAllGroupMessagesAdminAsync(cancellationToken);
        }
    }
}