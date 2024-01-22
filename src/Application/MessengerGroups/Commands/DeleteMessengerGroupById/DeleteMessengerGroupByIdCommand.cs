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

namespace PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupById
{
    public class DeleteMessengerGroupByIdCommand : IRequest
    {
        public Guid GroupId { get; set; }
    }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteMessengerGroupByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteGroupCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerGroupByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerGroups
                .Include(x => x.MessengerGroupUsers)
                .Include(x => x.MessengerGroupMessages)
                .FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            if (entity.OwnerId == _httpContextAccessor.HttpContext.GetUserId())
            {
                var listGroupUsers = entity.MessengerGroupUsers
                    .Where(x => x.MessengerGroupId == request.GroupId)
                    .ToList();

                if (listGroupUsers != null)
                {
                    foreach (var item in listGroupUsers)
                    {
                        _applicationDbContext.MessengerGroupUsers.Remove(item);
                    }
                }

                var listGroupMessages = await _applicationDbContext.MessengerGroupMessages
                    .Include(x => x.MessengerMessageFiles)
                    .Include(x => x.MessengerGroupMessageSeenMessages)
                    .Where(x => x.MessengerGroupId == request.GroupId)
                    .ToListAsync(cancellationToken);

                if (listGroupMessages != null)
                {
                    foreach (var item in listGroupMessages)
                    {
                        //_customUploadFileService.DeleteFile(item.MessengerMessageFiles.First().FileName);
                        _applicationDbContext.MessengerGroupMessages.Remove(item);
                    }
                }

                //if (!string.IsNullOrEmpty(entity.Image))
                //{
                //    _customUploadFileService.DeleteFile("MessengerGroups", entity.Image);
                //}

                _applicationDbContext.MessengerGroups.Remove(entity);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new Exception("شما نمی توانید این گروه را حذف کنید");

            return Unit.Value;
        }
    }
}
