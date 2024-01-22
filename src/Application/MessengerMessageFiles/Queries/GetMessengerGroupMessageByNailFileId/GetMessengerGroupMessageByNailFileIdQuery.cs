using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;

namespace PortalCore.Application.MessengerMessageFiles.Queries.GetMessengerGroupMessageByNailFileId
{
    public class GetMessengerGroupMessageByNailFileIdQuery : IRequest<MessengerDisplayFileDto>
    {
        public Guid Id { get; set; }
    }

    public class GetMessengerGroupMessageByNailFileIdQueryHandler : IRequestHandler<GetMessengerGroupMessageByNailFileIdQuery, MessengerDisplayFileDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GetMessengerGroupMessageByNailFileIdQueryHandler(IApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<MessengerDisplayFileDto> Handle(GetMessengerGroupMessageByNailFileIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.MessengerMessageFiles
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new NotFoundException();

            if (!string.IsNullOrEmpty(entity.FileThumbNail))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "MessengerGroupMessagesThumbNail", entity.FileThumbNail);
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