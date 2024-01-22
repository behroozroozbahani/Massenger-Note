using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayAllPrivateMessagesAdminDto
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string LastMessageDateTime { get; set; } = null!;
        public bool IsPrivateMessage { get; set; }
    }
}
