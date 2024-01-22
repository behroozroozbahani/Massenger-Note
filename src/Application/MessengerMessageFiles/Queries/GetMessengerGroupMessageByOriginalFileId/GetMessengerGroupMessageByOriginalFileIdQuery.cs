﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerGroupMessageByOriginalFileId
{
    public class GetMessengerGroupMessageByOriginalFileIdQuery : IRequest<MessengerDisplayFileDto>
    {
        public Guid Id { get; set; }
    }

    public class GetMessengerGroupMessageByOriginalFileIdQueryHandler : IRequestHandler<GetMessengerGroupMessageByOriginalFileIdQuery, MessengerDisplayFileDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMessengerGroupMessageByOriginalFileIdQueryHandler(IApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MessengerDisplayFileDto> Handle(GetMessengerGroupMessageByOriginalFileIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerMessageFiles
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException();

            if (!string.IsNullOrEmpty(entity.FileName))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "MessengerGroupMessages", entity.FileName);
                var file = File.OpenRead(path);

                var mimeType = MimeKit.MimeTypes.GetMimeType(file.Name);

                return new MessengerDisplayFileDto
                {
                    ContentType = mimeType,
                    FileContents = file,
                    FileDownloadName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name)
                };
            }
            else
                throw new NotFoundException();
        }
    }
}
