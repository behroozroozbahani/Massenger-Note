using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<IList<MessengerDisplayAllUsersDto>>
    {

    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IList<MessengerDisplayAllUsersDto>>
    {
        private readonly IMessengerAllUsersService _messengerAllUsers;

        public GetAllUsersQueryHandler(IMessengerAllUsersService messengerAllUsers)
        {
            _messengerAllUsers = messengerAllUsers;
        }

        public async Task<IList<MessengerDisplayAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _messengerAllUsers.GetAllUsersAsync(cancellationToken);
        }
    }
}