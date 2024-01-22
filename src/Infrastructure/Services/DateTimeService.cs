using System;
using PortalCore.Application.Common.Interfaces;

namespace PortalCore.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
