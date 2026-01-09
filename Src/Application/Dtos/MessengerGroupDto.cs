using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerGroupDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string OwnerFullName { get; set; } = null!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid OwnerId { get; set; }
    }
}
