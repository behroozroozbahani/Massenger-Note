using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.MessengerPrivateMessages.Commands.DeleteMessengerPrivateMessageAdminById
{
    public class DeleteMessengerPrivateMessageAdminByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteMessengerPrivateMessageAdminByIdCommandHandler : IRequestHandler<DeleteMessengerPrivateMessageAdminByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteMessengerPrivateMessageAdminByIdCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerPrivateMessageAdminByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.MessengerMessageFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            var role = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (role)
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
