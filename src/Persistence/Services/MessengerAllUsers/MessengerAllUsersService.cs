using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using PortalCore.Domain.Entities.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PortalCore.Application.Common.Exceptions;

namespace PortalCore.Persistence.Services.MessengerAllUsers
{
    public class MessengerAllUsersService : IMessengerAllUsersService
    {
        private readonly UserManager<User> _userManager;

        public MessengerAllUsersService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IList<MessengerDisplayAllUsersDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var list = await _userManager.Users
                .ToListAsync(cancellationToken);

            if (list is null)
                throw new NotFoundException();

            return list.Select(x => new MessengerDisplayAllUsersDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                ProfileImage = x.ProfileImage

            }).ToList();
        }
    }
}
