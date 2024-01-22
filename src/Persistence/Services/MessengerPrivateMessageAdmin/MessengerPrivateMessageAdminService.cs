using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageAdminBySenderRecipientId;
using PortalCore.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Persistence.Services.MessengerPrivateMessageAdmin
{
    public class MessengerPrivateMessageAdminService : IMessengerPrivateMessageAdminService
    {

        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessengerPrivateMessageAdminService(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<MessengerDisplayAllPrivateMessagesAdminDto>> GetAllPrivateMessagesAdminAsync(CancellationToken cancellationToken)
        {
            var list = await _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .ToListAsync(cancellationToken);

            return list.GroupBy(x => x.SenderRecipientId)
                       .Select(x => new MessengerDisplayAllPrivateMessagesAdminDto()
                       {
                           Id = x.First().Id,

                           SenderName = x.First().Sender.ToString(),

                           RecipientName = x.First().Recipient.ToString(),

                           LastMessageDateTime = list
                    .Where(z => z.SenderRecipientId == x.First().SenderRecipientId)
                    .OrderBy(c => c.SendDateTime)
                    .LastOrDefault()
                    .SendDateTime
                    .ToShortPersianDateTimeString(),

                           IsPrivateMessage = true

                       }).ToList();
        }

        public async Task<IEnumerable<MessengerDisplayPrivateMessageSenderRecipientDto>> GetPrivateMessageBySenderRecipientIdAdminAsync(Guid senderRecipientId, CancellationToken cancellationToken)
        {
            var request = new GetMessengerPrivateMessageAdminBySenderRecipientIdQuery()
            {
                SenderRecipientId = senderRecipientId
            };

            var list = _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.ParentMessage)
                .Where(x => x.SenderRecipientId == request.SenderRecipientId);

            var userHasMessage = list.Any(x => x.SenderId == _httpContextAccessor.HttpContext.GetUserId());

            if (list.Any(x => x.SenderId == _httpContextAccessor.HttpContext.GetUserId() ||
                              x.RecipientId == _httpContextAccessor.HttpContext.GetUserId()))
            {
                foreach (var item in list.Where(x => !x.IsRead))
                {
                    item.IsRead = true;
                    _applicationDbContext.MessengerPrivateMessages.Update(item);
                }
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return await list.ProjectTo<MessengerDisplayPrivateMessageSenderRecipientDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        }
    }
}
