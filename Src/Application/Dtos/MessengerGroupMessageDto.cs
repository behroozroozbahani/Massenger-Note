using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalCore.Application.Dtos
{
    public class MessengerGroupMessageDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; } = null!;
        public string SendDate { get; set; } = null!;
    }
}
