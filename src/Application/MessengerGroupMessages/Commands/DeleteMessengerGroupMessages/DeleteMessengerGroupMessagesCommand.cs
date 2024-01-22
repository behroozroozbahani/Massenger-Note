using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroupMessages.Commands.DeleteMessengerGroupMessages
{
    public class DeleteMessengerGroupMessagesCommand : IRequest
    {
        public List<Guid> MessageIds { get; set; } = null!;
    }

    public class DeleteMessengerGroupMessagesCommandHandler : IRequestHandler<DeleteMessengerGroupMessagesCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteMessengerGroupMessagesCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerGroupMessagesCommand request, CancellationToken cancellationToken)
        {
            var messages = await _applicationDbContext.MessengerGroupMessages
                .Include(x => x.MessengerGroup)
                .ThenInclude(x => x.MessengerGroupUsers)
                .Include(x => x.MessengerMessageFiles)
                .Include(x => x.MessengerGroupMessageSeenMessages)
                .Where(x => request.MessageIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            if (messages is null || !messages.Any())
                throw new NotFoundException();

            foreach (var message in messages)
            {
                if (message.SenderId != _httpContextAccessor.HttpContext.GetUserId() &&
                    message.MessengerGroup.OwnerId != _httpContextAccessor.HttpContext.GetUserId() &&
                    (!message.MessengerGroup.MessengerGroupUsers.Any(x =>
                        x.UserId == _httpContextAccessor.HttpContext.GetUserId() && x.IsAdmin)))
                    throw new UnauthorizedAccessException();
            }

            try
            {
                foreach (var message in messages)
                {
                    //foreach (var file in message.MessengerMessageFiles)
                    //{
                    //    _customUploadFileService.DeleteFile(file.FileName);
                    //}
                    _applicationDbContext.MessengerGroupMessageSeenMessages.RemoveRange(message
                        .MessengerGroupMessageSeenMessages);
                    _applicationDbContext.MessengerMessageFiles.RemoveRange(message.MessengerMessageFiles);
                    _applicationDbContext.MessengerGroupMessages.Remove(message);
                }
            }
            finally
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }


            return Unit.Value;
        }
    }
}
