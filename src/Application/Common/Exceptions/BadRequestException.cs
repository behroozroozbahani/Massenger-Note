using PortalCore.Application.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using System;

namespace PortalCore.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public string? CustomMessage { get; }

        public BadRequestException()
            : base()
        {
            CustomMessage = null;
        }

        public BadRequestException(string message)
            : base(message)
        {
            CustomMessage = message;
        }

        public BadRequestException(string message, Exception exception)
            : base(message, exception)
        {
            CustomMessage = message;
        }

        public BadRequestException(IdentityResult identityResult)
            : base()
        {
            var errors = identityResult.GetDescriptionErrors();
            CustomMessage = string.Join(Environment.NewLine, errors);
        }
    }
}
