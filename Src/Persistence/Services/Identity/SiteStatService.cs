using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PortalCore.Application.Common.Identity;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace PortalCore.Persistence.Services.Identity
{
    public class SiteStatService : ISiteStatService
    {
        private readonly IApplicationDbContext _uow;
        private readonly IApplicationUserManager _userManager;
        private readonly DbSet<User> _users;

        public SiteStatService(
            IApplicationUserManager userManager,
            IApplicationDbContext uow)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _users = uow.Set<User>();
        }

        public Task<List<User>> GetOnlineUsersListAsync(int numbersToTake, int minutesToTake)
        {
            var now = DateTime.UtcNow;
            var minutes = now.AddMinutes(-minutesToTake);
            return _users.AsNoTracking()
                         .Where(user => user.LastVisitDateTime != null && user.LastVisitDateTime.Value <= now
                                        && user.LastVisitDateTime.Value >= minutes)
                         .OrderByDescending(user => user.LastVisitDateTime)
                         .Take(numbersToTake)
                         .ToListAsync();
        }

        public async Task UpdateUserLastVisitDateTimeAsync(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);
            user.LastVisitDateTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
    }
}