using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTPersianUtils.Core;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupId;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalCore.Common.Extensions;

namespace PortalCore.Persistence.Services.MessengerGroupMessage
{
    public class MessengerGroupMessageService : IMessengerGroupMessageService
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessengerGroupMessageService(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        //نمایش تمامی گروه های عضو برای کاربر لاگین شده
        public async Task<IList<MessengerDisplayAllGroupMessagesDto>> GetAllGroupMessagesAsync(CancellationToken cancellationToken)
        {
            //var list = await _applicationDbContext.MessengerGroupMessages
            //         .Include(x => x.Sender)
            //         .Include(x => x.MessengerGroup)
            //         .ThenInclude(x => x.MessengerGroupUsers)
            //         .Include(x => x.MessengerMessageFiles)
            //         .Include(x => x.MessengerGroupMessageSeenMessages)
            //         .Where(x => x.SenderId == Guid.Parse("65c33454-fccc-4c48-228a-08d9c7a82ae1") ||
            //                     x.MessengerGroup.MessengerGroupUsers
            //                         .Any(d => d.UserId == Guid.Parse("65c33454-fccc-4c48-228a-08d9c7a82ae1")))
            //         .ToListAsync(cancellationToken);

            var list = await _applicationDbContext.MessengerGroups
                .Include(x => x.MessengerGroupMessages)
                .ThenInclude(x => x.MessengerGroupMessageSeenMessages)
                .Include(x => x.MessengerGroupUsers)
                .Where(x => (x.OwnerId == _httpContextAccessor.HttpContext.GetUserId() ||
                             x.MessengerGroupUsers
                                 .Any(z => z.UserId == _httpContextAccessor.HttpContext.GetUserId())) &&
                            x.IsActive)
                .ToListAsync(cancellationToken);

            //list = list
            //    .OrderBy(x => x.MessengerGroupMessages
            //        .OrderBy(z => z.SendDateTime))
            //    .ToList();

            var groupMessageList = list
                .Select(x => x.MessengerGroupMessages
                    .Select(z => z.Id)
                    .ToList())
                .ToList();

            var seenMessageList = list
                .Select(x => x.MessengerGroupMessages
                    .Where(z => z.MessengerGroupMessageSeenMessages
                        .Any(c => c.UserId == _httpContextAccessor.HttpContext.GetUserId()))
                    .Select(y => y.Id).ToList()).ToList();

            return list.Select(x => new MessengerDisplayAllGroupMessagesDto()
            {
                Id = x.Id,

                MessageBody = x.MessengerGroupMessages
                    .OrderBy(t => t.SendDateTime)
                    .LastOrDefault().MessageBody,

                Image = x.Image,

                Name = x.Name,

                LastMessageDateTime = x.MessengerGroupMessages
                    .OrderBy(t => t.SendDateTime)
                    .LastOrDefault().SendDateTime.ToShortPersianDateTimeString(),

                CountMessageUnRead = groupMessageList.Except(seenMessageList).Count(),

                IsGroupMessage = true

            }).ToList();
        }

        //نمایش تمامی پیام های گروه خاص برای کاربر لاگین شده
        public async Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdDto>> GetGroupMessagesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var request = new GetMessengerGroupMessagesByGroupIdQuery()
            {
                GroupId = groupId
            };

            var list = _applicationDbContext.MessengerGroupMessages
                .Include(x => x.ParentMessage)
                .Where(t => t.MessengerGroupId == request.GroupId);

            var unSeenMessage = await list.Where(x => x.MessengerGroupMessageSeenMessages
                                                          .All(c => c.UserId != _httpContextAccessor.HttpContext.GetUserId()) &&
                                                      x.SenderId != _httpContextAccessor.HttpContext.GetUserId())
                .Select(n => n.Id)
                .ToListAsync(cancellationToken);

            foreach (var item in unSeenMessage)
            {
                _applicationDbContext.MessengerGroupMessageSeenMessages.Add(new MessengerGroupMessageSeenMessage()
                {
                    MessengerGroupMessageId = item,
                    UserId = _httpContextAccessor.HttpContext.GetUserId(),
                });
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return await list.ProjectTo<MessengerDisplayGroupMessagesByGroupIdDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        }
    }
}