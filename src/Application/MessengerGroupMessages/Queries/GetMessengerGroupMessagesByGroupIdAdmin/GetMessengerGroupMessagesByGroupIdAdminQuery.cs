using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupIdAdmin
{
    public class GetMessengerGroupMessagesByGroupIdAdminQuery : IRequest<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>>
    {
        public Guid GroupId { get; set; }
    }

    public class GetMessengerGroupMessagesByGroupIdAdminQueryHandler : IRequestHandler<GetMessengerGroupMessagesByGroupIdAdminQuery, IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>>
    {
        private readonly IMessengerGroupMessageAdminService _messengerGroupMessageAdminService;

        public GetMessengerGroupMessagesByGroupIdAdminQueryHandler(IMessengerGroupMessageAdminService messengerGroupMessageAdminService)
        {
            _messengerGroupMessageAdminService = messengerGroupMessageAdminService;
        }

        public async Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>> Handle(GetMessengerGroupMessagesByGroupIdAdminQuery request, CancellationToken cancellationToken)
        {
            return await _messengerGroupMessageAdminService.GetGroupMessageByGroupIdAdminAsync(request.GroupId, cancellationToken);
        }
    }
}