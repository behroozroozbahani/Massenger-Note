using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CheckExistPhoneNumber(string phoneNumber,CancellationToken cancellationToken);
        bool IsValidNationalCode(string nationalCode);
        bool IsValidPhoneNumber(string phoneNumber);
    }
}
