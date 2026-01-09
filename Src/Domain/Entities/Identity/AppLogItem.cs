using System;
using PortalCore.Domain.Common;

namespace PortalCore.Domain.Entities.Identity
{
    public class AppLogItem : IEntity, IAuditableEntity
    {
        public Guid Id { set; get; }

        public DateTimeOffset? CreatedDateTime { get; set; }

        public int EventId { get; set; }

        public string? Url { get; set; }

        public string? LogLevel { get; set; }

        public string? Logger { get; set; }

        public string? Message { get; set; }

        public string? StateJson { get; set; }
    }
}