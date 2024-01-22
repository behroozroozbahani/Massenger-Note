using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupMemberById
{
    public class DeleteMessengerGroupMemberByIdCommand : IRequest
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteMessengerGroupMemberByIdCommandHandler : IRequestHandler<DeleteMessengerGroupMemberByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteMessengerGroupMemberByIdCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerGroupMemberByIdCommand request, CancellationToken cancellationToken)
        {
            var groupOwner = await _applicationDbContext.MessengerGroupUsers
                .Include(x => x.MessengerGroup)
                .FirstOrDefaultAsync(x => (x.MessengerGroupId == request.GroupId &&
                                           x.UserId == _httpContextAccessor.HttpContext.GetUserId() &&
                                           x.IsAdmin == true)
                                          ||
                                          (x.MessengerGroup.OwnerId == _httpContextAccessor.HttpContext.GetUserId() &&
                                           x.MessengerGroup.Id == request.GroupId), cancellationToken);
            if (groupOwner is null)
                throw new UnauthorizedAccessException();

            var user = await _applicationDbContext.MessengerGroupUsers
                .FirstOrDefaultAsync(x => x.UserId == request.UserId &&
                                          x.MessengerGroupId == request.GroupId, cancellationToken);

            if (user != null)
            {
                _applicationDbContext.MessengerGroupUsers.Remove(user);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new NotFoundException();

            return Unit.Value;
        }
    }
}
