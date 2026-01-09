using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroupMessages.Commands.DeleteMessengerGroupMessageById
{
    public class DeleteMessengerGroupMessageByIdCommand : IRequest
    {
        public Guid MessageId { get; set; }
    }

    public class DeleteMessengerGroupMessageByIdCommandHandler : IRequestHandler<DeleteMessengerGroupMessageByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteMessengerGroupMessageByIdCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerGroupMessageByIdCommand request, CancellationToken cancellationToken)
        {
            var message = await _applicationDbContext.MessengerGroupMessages
                .Include(x => x.MessengerGroup)
                .ThenInclude(x => x.MessengerGroupUsers)
                .Include(x => x.MessengerMessageFiles)
                .Include(x => x.MessengerGroupMessageSeenMessages)
                .FirstOrDefaultAsync(x => x.Id == request.MessageId, cancellationToken);

            if (message is null)
                throw new NotFoundException();

            if (message.SenderId == _httpContextAccessor.HttpContext.GetUserId() ||
                message.MessengerGroup.OwnerId == _httpContextAccessor.HttpContext.GetUserId() ||
                (message.MessengerGroup.MessengerGroupUsers.Any(x =>
                    x.UserId == _httpContextAccessor.HttpContext.GetUserId() && x.IsAdmin)))
            {
                //foreach (var file in message.MessengerMessageFiles)
                //{
                //    _customUploadFileService.DeleteFile(file.FileName);
                //}

                _applicationDbContext.MessengerGroupMessageSeenMessages.RemoveRange(message
                    .MessengerGroupMessageSeenMessages);
                _applicationDbContext.MessengerMessageFiles.RemoveRange(message.MessengerMessageFiles);
                _applicationDbContext.MessengerGroupMessages.Remove(message);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}