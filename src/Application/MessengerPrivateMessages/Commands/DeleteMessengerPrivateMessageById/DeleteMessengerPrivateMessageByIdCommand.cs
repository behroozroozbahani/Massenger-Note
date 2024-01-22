using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerPrivateMessages.Commands.DeleteMessengerPrivateMessageById
{
    public class DeleteMessengerPrivateMessageByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeletePrivateMessageByIdCommandHandler : IRequestHandler<DeleteMessengerPrivateMessageByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DeletePrivateMessageByIdCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerPrivateMessageByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.MessengerMessageFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            if (entity.SenderId == _httpContextAccessor.HttpContext.GetUserId() &&
                entity.Id == request.Id ||
                entity.RecipientId == _httpContextAccessor.HttpContext.GetUserId() &&
                entity.Id == request.Id)
            {

                if (entity.MessengerMessageFiles.Any())
                {
                    foreach (var file in entity.MessengerMessageFiles)
                    {
                        //_customUploadFileService.DeleteFile(file.FileName);
                        _applicationDbContext.MessengerMessageFiles.Remove(file);
                    }
                }

                _applicationDbContext.MessengerPrivateMessages.Remove(entity);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}