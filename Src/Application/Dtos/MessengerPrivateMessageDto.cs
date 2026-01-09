using System;
using System.Collections.Generic;

namespace PortalCore.Application.Dtos
{
    public class MessengerPrivateMessageDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string SendDate { get; set; } = null!;
        public IEnumerable<MessengerPrivateMessageDto>? MessengerPrivateMessageDtos { get; set; }
    }
}
