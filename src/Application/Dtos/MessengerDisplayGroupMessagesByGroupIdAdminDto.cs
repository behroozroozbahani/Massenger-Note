using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayGroupMessagesByGroupIdAdminDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string SendDate { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public string? ParentMessage { get; set; }
    }
}
