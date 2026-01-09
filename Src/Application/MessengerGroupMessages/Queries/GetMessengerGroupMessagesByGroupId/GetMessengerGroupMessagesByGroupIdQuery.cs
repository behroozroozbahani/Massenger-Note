using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupId
{
    public class GetMessengerGroupMessagesByGroupIdQuery : IRequest<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>>
    {
        public Guid GroupId { get; set; }
    }

    public class GetMessengerGroupMessagesByGroupIdQueryHandler : IRequestHandler<GetMessengerGroupMessagesByGroupIdQuery, IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>>
    {
        private readonly IMessengerGroupMessageService _messengerGroupMessageService;

        public GetMessengerGroupMessagesByGroupIdQueryHandler(IMessengerGroupMessageService messengerGroupMessageService)
        {
            _messengerGroupMessageService = messengerGroupMessageService;
        }

        public async Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>> Handle(GetMessengerGroupMessagesByGroupIdQuery request, CancellationToken cancellationToken)
        {
            return await _messengerGroupMessageService.GetGroupMessagesByGroupIdAsync(request.GroupId, cancellationToken);
        }
    }
}