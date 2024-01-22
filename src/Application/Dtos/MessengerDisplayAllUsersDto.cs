using System;

namespace PortalCore.Application.Dtos
{
    public class MessengerDisplayAllUsersDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImage { get; set; }
    }
}
