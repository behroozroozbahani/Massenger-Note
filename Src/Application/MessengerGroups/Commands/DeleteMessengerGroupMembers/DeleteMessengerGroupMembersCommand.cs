using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroups.Commands.DeleteMessengerGroupMembers
{
    public class DeleteMessengerGroupMembersCommand : IRequest
    {
        public Guid GroupId { get; set; }
        public List<Guid> UserIds { get; set; } = null!;
    }

    public class DeleteMessengerGroupMembersCommandHandler : IRequestHandler<DeleteMessengerGroupMembersCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteMessengerGroupMembersCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteMessengerGroupMembersCommand request, CancellationToken cancellationToken)
        {
            if (request.UserIds is null)
                throw new Exception("شما هنوز عضوی را انتخاب نکرده اید");

            var groupOwner = await _applicationDbContext.MessengerGroupUsers
                .Include(x => x.MessengerGroup)
                .FirstOrDefaultAsync(x => (x.MessengerGroupId == request.GroupId &&
                                           x.UserId == _httpContextAccessor.HttpContext.GetUserId() &&
                                           x.IsAdmin == true)
                                          ||
                                          (x.MessengerGroup.OwnerId ==
                                           _httpContextAccessor.HttpContext.GetUserId() &&
                                           x.MessengerGroup.Id == request.GroupId), cancellationToken);

            if (groupOwner is null)
                throw new UnauthorizedAccessException();

            var listMembers = await _applicationDbContext.MessengerGroupUsers
                .Where(x => request.UserIds.Contains(x.UserId))
                .ToListAsync(cancellationToken);

            if (listMembers.Any())
            {
                _applicationDbContext.MessengerGroupUsers.RemoveRange(listMembers);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new NotFoundException();

            return Unit.Value;
        }
    }
}
