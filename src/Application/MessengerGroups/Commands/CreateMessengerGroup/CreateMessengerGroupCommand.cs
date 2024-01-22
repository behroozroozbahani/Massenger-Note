 using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Cms;
using PortalCore.Common.Extensions;

namespace PortalCore.Application.MessengerGroups.Commands.CreateMessengerGroup
{
    public class CreateMessengerGroupCommand : IRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public List<Guid> UserIds { get; set; } = null!;
    }

    public class CreateGroupCommandHandler : IRequestHandler<CreateMessengerGroupCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMessengerCheckImagesService _messengerCheckImagesService;
        private readonly IThumbnailService _thumbnailService;
        private readonly IGenerateFileNameService _generateFileNameService;

        public CreateGroupCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor, IMessengerCheckImagesService messengerCheckImagesService, IThumbnailService thumbnailService, IGenerateFileNameService generateFileNameService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
            _messengerCheckImagesService = messengerCheckImagesService;
            _thumbnailService = thumbnailService;
            _generateFileNameService = generateFileNameService;
        }

        public async Task<Unit> Handle(CreateMessengerGroupCommand request, CancellationToken cancellationToken)
        {
            string? chechFileMultipel = null;

            if (request.Image != null)
            {
                _messengerCheckImagesService.CheckFileAsync(request.Image, cancellationToken);
                chechFileMultipel =
                    _messengerCheckImagesService.ChechFileMultipelExtension(request.Image.FileName, cancellationToken);
            }

            var entity = _mapper.Map<MessengerGroup>(request);

            //User logged in
            entity.OwnerId = _httpContextAccessor.HttpContext.GetUserId();

            if (request.Name is null)
                throw new Exception("گروه بدون نام نمی تواند ایجاد شود");

            if (request.UserIds is null)
                throw new Exception("گروه یا تعداد اعضای صفر نمی تواند ایجاد شود");

            entity.MessengerGroupUsers.Add(new MessengerGroupUser()
            {
                Id = Guid.NewGuid(),
                UserId = entity.OwnerId,
                IsAdmin = true
            });

            if (chechFileMultipel != null)
            {
                var saveFileResult = await _customUploadFileService
                    .UploadFileAsync("MessengerGroups", request.Image);

                entity.Image = saveFileResult.FileName;
            }

            foreach (var item in request.UserIds)
            {
                entity.MessengerGroupUsers.Add(new MessengerGroupUser()
                {
                    Id = Guid.NewGuid(),
                    UserId = item,
                });
            }

            await _applicationDbContext.MessengerGroups.AddAsync(entity, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}