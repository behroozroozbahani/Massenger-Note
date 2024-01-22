using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayAllPrivateMessagesDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string? ProfileImage { get; set; }
        public string FullName { get; set; } = null!;
        public string LastMessageDateTime { get; set; }
        public int CountMessageUnRead { get; set; }
        public bool IsPrivateMessage { get; set; }
    }
}
