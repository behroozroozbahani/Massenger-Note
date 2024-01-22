using System;
using System.Threading.Tasks;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Common.Identity
{
    public interface IUsedPasswordsService
    {
        Task<bool> IsPreviouslyUsedPasswordAsync(User user, string newPassword);
        Task AddToUsedPasswordsListAsync(User user);
        Task<bool> IsLastUserPasswordTooOldAsync(Guid userId);
        Task<DateTime?> GetLastUserPasswordChangeDateAsync(Guid userId);
    }
}