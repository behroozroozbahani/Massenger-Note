using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroupNewMember
{
    public class CreateMessengerGroupNewMemberCommand : IRequest
    {
        public Guid GroupId { get; set; }
        public List<Guid> UserIds { get; set; } = null!;
    }

    public class CreateMessengerGroupNewMemberCommandHandler : IRequestHandler<CreateMessengerGroupNewMemberCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateMessengerGroupNewMemberCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(CreateMessengerGroupNewMemberCommand request, CancellationToken cancellationToken)
        {
            if (request.UserIds is null)
                throw new Exception("شما هنوز عضوی را انتخاب نکرده اید");

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

            var listDuplicateMembers = await _applicationDbContext
                .Set<MessengerGroupUser>()
                .AsQueryable()
                .Where(x => request.UserIds.Contains(x.UserId) && x.MessengerGroupId == request.GroupId)
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken);

            var listMembersMustAddToDb = request.UserIds
                .Except(listDuplicateMembers)
                .ToList();

            foreach (var item in listMembersMustAddToDb)
            {
                _applicationDbContext.MessengerGroupUsers.Add(new MessengerGroupUser()
                {
                    Id = Guid.NewGuid(),
                    UserId = item,
                    MessengerGroupId = request.GroupId
                });
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
