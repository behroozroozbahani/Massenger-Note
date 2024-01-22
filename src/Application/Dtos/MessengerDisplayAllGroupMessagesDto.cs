using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayAllGroupMessagesDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public string LastMessageDateTime { get; set; } = null!;
        public int CountMessageUnRead { get; set; }
        public bool IsGroupMessage { get; set; }
    }
}
