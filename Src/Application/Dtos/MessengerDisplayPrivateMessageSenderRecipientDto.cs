using Microsoft.AspNetCore.Mvc;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayPrivateMessageSenderRecipientDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string SendDate { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public string? ParentMessage { get; set; }
        public bool IsRead { get; set; }
    }
}
