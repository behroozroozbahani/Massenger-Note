using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerMessageFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
    }
}
