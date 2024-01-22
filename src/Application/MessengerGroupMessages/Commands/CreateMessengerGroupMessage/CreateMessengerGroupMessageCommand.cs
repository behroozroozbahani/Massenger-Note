using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Common.Extensions;
using PortalCore.Domain.Entities;

namespace PortalCore.Application.MessengerGroupMessages.Commands.CreateMessengerGroupMessage
{
    public class CreateMessengerGroupMessageCommand : IRequest
    {
        public Guid GroupId { get; set; }
        public string MessageBody { get; set; } = null!;
        public Guid? ParentId { get; set; }

        //Multi File
        //public List<IFormFile>? Files { get; set; }

        //Single File
        public IFormFile? File { get; set; }
    }

    public class CreateMessengerGroupMessageCommandHandler : IRequestHandler<CreateMessengerGroupMessageCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessengerCheckImagesService _messengerCheckImagesService;
        private readonly IThumbnailService _thumbnailService;
        private readonly IGenerateFileNameService _generateFileNameService;

        public CreateMessengerGroupMessageCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor, IMessengerCheckImagesService messengerCheckImagesService, IThumbnailService thumbnailService, IGenerateFileNameService generateFileNameService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
            _messengerCheckImagesService = messengerCheckImagesService;
            _thumbnailService = thumbnailService;
            _generateFileNameService = generateFileNameService;
        }

        public async Task<Unit> Handle(CreateMessengerGroupMessageCommand request, CancellationToken cancellationToken)
        {
            string? chechFileMultipel = null;

            if (request.File != null)
            {
                _messengerCheckImagesService.CheckFileAsync(request.File, cancellationToken);
                chechFileMultipel = _messengerCheckImagesService.ChechFileMultipelExtension(request.File.FileName, cancellationToken);
            }

            var entity = _mapper.Map<MessengerGroupMessage>(request);

            if (entity is null)
                throw new NotFoundException();

            //User logged in
            entity.SenderId = _httpContextAccessor.HttpContext.GetUserId();

            var listGroupMembers =
                _applicationDbContext.MessengerGroupUsers
                    .Where(x => x.MessengerGroupId == request.GroupId)
                    .ToList();

            //if (listGroupMembers.Count == 0)
            //    throw new NotFoundException();

                var result = listGroupMembers
                .Find(x => x.UserId == entity.SenderId);

            //var listIsAdmins = listGroupMembers
            //    .Where(x => x.IsAdmin == true)
            //    .ToList();

            if (result != null)
            {
                if (result.IsAdmin)
                {
                    entity.IsAdmin = true;
                }

                //Multi File
                //if (request.Files != null && request.Files.Any())
                //{
                //    foreach (var file in request.Files)
                //    {
                //        var saveFileResult = await _customUploadFileService.UploadFileAsync(file, "MessengerMessages");

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
                    var saveFileResult = await _customUploadFileService.UploadFileAsync("MessengerGroupMessages", request.File);
                    var saveThumbResult = await _thumbnailService.CreateThumbnailAsync(request.File, 256, "MessengerGroupMessagesThumbNail", request.File.FileName, cancellationToken);
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

                await _applicationDbContext.MessengerGroupMessages.AddAsync(entity, cancellationToken);

                await _applicationDbContext.MessengerGroupMessageSeenMessages.AddAsync(
                    new MessengerGroupMessageSeenMessage()
                    {
                        MessengerGroupMessageId = entity.Id,
                        UserId = entity.SenderId
                    }, cancellationToken);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new NotFoundException();

            return Unit.Value;
        }
    }
}