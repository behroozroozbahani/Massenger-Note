using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Persistence.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;

        public AccountService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CheckExistPhoneNumber(string phoneNumber, CancellationToken cancellationToken) => await _userManager.Users.AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);

        public bool IsValidNationalCode(string nationalCode)
        {
            var result = nationalCode.IsValidIranianNationalCode();
            return result;
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.IsValidIranianMobileNumber();
        }
    }
}
