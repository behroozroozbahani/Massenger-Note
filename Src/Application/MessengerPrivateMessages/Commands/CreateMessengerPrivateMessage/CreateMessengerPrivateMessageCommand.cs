using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using PortalCore.Domain.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.MessengerPrivateMessages.Commands.CreateMessengerPrivateMessage
{
    public class CreateMessengerPrivateMessageCommand : IRequest
    {
        public Guid RecipientId { get; set; }
        public Guid? ParentId { get; set; }
        public string MessageBody { get; set; } = null!;

        //Multi File
        //public List<IFormFile>? Files { get; set; }

        //Single File
        public IFormFile? File { get; set; }
    }

    public class CreatePrivateMessageCommandHandler : IRequestHandler<CreateMessengerPrivateMessageCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessengerCheckImagesService _messengerCheckImagesService;
        private readonly IThumbnailService _thumbnailService;
        private readonly IGenerateFileNameService _generateFileNameService;

        public CreatePrivateMessageCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor, IMessengerCheckImagesService messengerCheckImagesService, IThumbnailService thumbnailService, IGenerateFileNameService generateFileNameService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
            _messengerCheckImagesService = messengerCheckImagesService;
            _thumbnailService = thumbnailService;
            _generateFileNameService = generateFileNameService;
        }

        public async Task<Unit> Handle(CreateMessengerPrivateMessageCommand request, CancellationToken cancellationToken)
        {
            string? chechFileMultipel = null;

            if (request.File != null)
            {
                _messengerCheckImagesService.CheckFileAsync(request.File, cancellationToken);
                chechFileMultipel = _messengerCheckImagesService.ChechFileMultipelExtension(request.File.FileName, cancellationToken);
            }

            var entity = _mapper.Map<MessengerPrivateMessage>(request);

            if (entity is null)
                throw new NotFoundException();

            //Sender Message
            entity.SenderId = _httpContextAccessor.HttpContext.GetUserId();

            var senderRecipientId = _applicationDbContext.MessengerPrivateMessages
                .FirstOrDefault(x =>
                    (x.SenderId == _httpContextAccessor.HttpContext.GetUserId() &&
                    x.RecipientId == request.RecipientId) ||
                    (x.RecipientId == _httpContextAccessor.HttpContext.GetUserId() &&
                    x.SenderId == request.RecipientId));

            if (senderRecipientId is null)
            {
                entity.SenderRecipientId = Guid.NewGuid();
            }
            else
            {
                entity.SenderRecipientId = senderRecipientId.SenderRecipientId;
            }

            //Multi File
            //if (request.Files != null && request.Files.Any())
            //{
            //    foreach (var file in request.Files)
            //    {
            //        var saveFileResult = await _customUploadFileService.UploadFileAsync(file, "MessengerPrivateMessages");

            //        entity.MessengerMessageFiles.Add(new MessengerMessageFile()
            //        {
            //            Id = Guid.NewGuid(),
            //            FileName = saveFileResult.FilePath
            //        });
            //    }
            //}

            //Single File
            if (chechFileMultipel != null)
            {
                var saveFileResult = await _customUploadFileService.UploadFileAsync("MessengerPrivateMessages", request.File);
                var saveThumbResult = await _thumbnailService.CreateThumbnailAsync(request.File, 256, "MessengerPrivateMessagesThumbNail", request.File.FileName, cancellationToken);
                entity.MessengerMessageFiles.Add(new MessengerMessageFile()
                {
                    Id = Guid.NewGuid(),
                    FileName = saveFileResult.FileName,
                    FilePath = saveFileResult.FilePath,
                    FileExtension = Path.GetExtension(saveFileResult.FileName),
                    FileMime = MimeKit.MimeTypes.GetMimeType(saveFileResult.FileName),
                    //FileRealName = request.File.FileName.Replace(Path.GetExtension(request.File.FileName), ""),
                    FileRealName = chechFileMultipel.Replace(Path.GetExtension(chechFileMultipel), ""),
                    FileThumbNail = saveThumbResult
                });
            }

            await _applicationDbContext.MessengerPrivateMessages.AddAsync(entity, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}