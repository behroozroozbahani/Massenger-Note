using DNTPersianUtils.Core;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using PortalCore.Application.MessengerGroupMessages.Queries.GetMessengerGroupMessagesByGroupIdAdmin;

namespace PortalCore.Persistence.Services.MessengerGroupMessageAdmin
{
    public class MessengerGroupMessageAdminService : IMessengerGroupMessageAdminService
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public MessengerGroupMessageAdminService(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        //نمایش تمامی گروه ها برای ادمین
        public async Task<IList<MessengerDisplayAllGroupMessagesAdminDto>> GetAllGroupMessagesAdminAsync(CancellationToken cancellationToken)
        {
            var list = await _applicationDbContext.MessengerGroups
                .Include(x => x.MessengerGroupMessages)
                .ToListAsync(cancellationToken);

            return list.Select(x => new MessengerDisplayAllGroupMessagesAdminDto()
            {
                Id = x.Id,

                MessageBody = x.MessengerGroupMessages.Any() ?
                              x.MessengerGroupMessages.OrderBy(t => t.SendDateTime).LastOrDefault().MessageBody : string.Empty,

                Image = x.Image,

                Name = x.Name,

                LastMessageDateTime = x.MessengerGroupMessages.Any() ?
                                      x.MessengerGroupMessages.OrderBy(t => t.SendDateTime).LastOrDefault().SendDateTime.ToShortPersianDateTimeString() : string.Empty,

                IsGroupMessage = true

            }).ToList();
        }

        //نمایش تمامی پیام های گروه خاص برای ادمین
        public async Task<IEnumerable<MessengerDisplayGroupMessagesByGroupIdAdminDto>> GetGroupMessageByGroupIdAdminAsync(Guid groupId, CancellationToken cancellationToken)
        {
            var request = new GetMessengerGroupMessagesByGroupIdAdminQuery()
            {
                GroupId = groupId
            };

            var list = _applicationDbContext.MessengerGroupMessages
                .Include(x => x.ParentMessage)
                .Where(x => x.MessengerGroupId == request.GroupId);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return await list.ProjectTo<MessengerDisplayGroupMessagesByGroupIdAdminDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        }
    }
}
