using System.Threading;
using System.Threading.Tasks;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace PortalCore.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
        where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            //string userName = string.Empty;

            //if (!userId.IsNullOrEmpty())
            //{
            //    userName = await _identityService.GetUserNameAsync(userId!.Value);
            //}

            _logger.LogInformation("PortalCore Request: {Name} {@UserId} {@Request}",
                requestName, userId, request);
        }
    }
}
