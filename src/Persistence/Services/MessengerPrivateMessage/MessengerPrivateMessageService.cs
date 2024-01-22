using AutoMapper;
using AutoMapper.QueryableExtensions;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using PortalCore.Application.MessengerPrivateMessages.Queries.GetMessengerPrivateMessageBySenderRecipientId;
using PortalCore.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace PortalCore.Persistence.Services.MessengerPrivateMessage
{
    public class MessengerPrivateMessageService : IMessengerPrivateMessageService
    {

        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MessengerPrivateMessageService(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IList<MessengerDisplayAllPrivateMessagesDto>> GetAllPrivateMessagesAsync(CancellationToken cancellationToken)
        {
            var list = await _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .Where(x =>
                    x.SenderId == _httpContextAccessor.HttpContext.GetUserId() ||
                    x.RecipientId == _httpContextAccessor.HttpContext.GetUserId())
                .ToListAsync(cancellationToken);

            return list.GroupBy(x => x.SenderRecipientId).Select(x => new MessengerDisplayAllPrivateMessagesDto()
            {
                Id = x.First().Id,

                MessageBody = x.First().MessageBody,

                ProfileImage = x.First().SenderId == _httpContextAccessor.HttpContext.GetUserId() ?
                    (x.First().Recipient.ProfileImage + " " + x.First().Recipient.ProfileImage) :
                    (x.First().Sender.ProfileImage + " " + x.First().Sender.ProfileImage),

                FullName = x.First().SenderId == _httpContextAccessor.HttpContext.GetUserId() ?
                    (x.First().Recipient.FirstName + " " + x.First().Recipient.LastName) :
                    (x.First().Sender.FirstName + " " + x.First().Sender.LastName),

                LastMessageDateTime = list
                    .Where(z => z.SenderRecipientId == x.First().SenderRecipientId)
                    .OrderBy(c => c.SendDateTime)
                    .LastOrDefault()
                    .SendDateTime
                    .ToShortPersianDateTimeString(),

                CountMessageUnRead = list
                    .Where(z => z.SenderRecipientId == x.First().SenderRecipientId)
                    .Count(c => !c.IsRead),

                IsPrivateMessage = true

            }).ToList();
        }

        public async Task<MessengerDisplayMessagesDto> GetPrivateMessageBySenderRecipientIdAsync(Guid senderRecipientId, CancellationToken cancellationToken)
        {
            var request = new GetMessengerPrivateMessageBySenderRecipientIdQuery()
            {
                SenderRecipientId = senderRecipientId
            };

            var messageList = await _applicationDbContext.MessengerPrivateMessages
                .Include(x => x.ParentMessage)
                .Include(x => x.MessengerMessageFiles)
                .Where(x => x.SenderRecipientId == request.SenderRecipientId)
                .ToListAsync(cancellationToken);

            var userHasMessage = messageList.Any();

            if (userHasMessage != true)
                throw new NotFoundException();

            foreach (var item in messageList.Where(x => !x.IsRead))
            {
                item.IsRead = true;
                _applicationDbContext.MessengerPrivateMessages.Update(item);
            }

            var result = new MessengerDisplayMessagesDto();
            var messageFilesResult = new MessengerDisplayMessagesDto();

            foreach (var item in messageList)
            {
                if (item.MessengerMessageFiles.Any())
                {
                    foreach (var messageFiles in item.MessengerMessageFiles)
                    {
                        if (!string.IsNullOrEmpty(messageFiles.FileThumbNail))
                        {
                            var path = Path.Combine(_webHostEnvironment.WebRootPath,
                                "MessengerPrivateMessagesThumbNail",
                                messageFiles.FileThumbNail);
                            var file = File.OpenRead(path);
                            var mimeType = MimeKit.MimeTypes.GetMimeType(file.Name);

                            messageFilesResult = new MessengerDisplayMessagesDto
                            {
                                ContentType = mimeType,
                                FileContents = file,
                                FileDownloadName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name)
                            };
                        }
                        else
                            throw new NotFoundException();
                    }

                    result = new MessengerDisplayMessagesDto()
                    {
                        Id = item.Id,
                        MessageBody = item.MessageBody,
                        SendDate = item.SendDateTime.ToShortPersianDateString(),
                        ParentId = item.ParentId,
                        ParentMessage = item.ParentMessage != null ? item.ParentMessage.ToString() : string.Empty,
                        IsRead = item.IsRead,
                        FileStreamResults = messageFilesResult.FileStreamResults
                    };
                }
            }

            return result;
        }
    }
}