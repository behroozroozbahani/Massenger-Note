using PortalCore.Application.Common.Models;
using PortalCore.Domain.Entities.Identity;

namespace PortalCore.Application.Common.Logger
{
    public class LoggerItem
    {
        public AppShadowProperties? Props { set; get; }
        public AppLogItem AppLogItem { set; get; } = null!;
    }
}
