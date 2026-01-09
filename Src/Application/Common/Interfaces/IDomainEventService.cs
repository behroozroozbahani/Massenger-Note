using System.Threading.Tasks;
using PortalCore.Domain.Common;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
