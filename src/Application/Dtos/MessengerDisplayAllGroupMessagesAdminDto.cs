using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayAllGroupMessagesAdminDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public string LastMessageDateTime { get; set; } = null!;
        public bool IsGroupMessage { get; set; }
    }
}
