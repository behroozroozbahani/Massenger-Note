using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Notes.Commands.CreateNoteAdmin
{
    public class CreateNoteAdminCommand : IRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public List<IFormFile>? Files { get; set; }

        //Single File
        //public IFormFile? File { get; set; }
    }

    public class CreateNoteAdminCommandHandler : IRequestHandler<CreateNoteAdminCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateNoteAdminCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(CreateNoteAdminCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Note>(request);

            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            //Admin
            if (rol)
            {
                if (request.Files != null && request.Files.Any())
                {
                    foreach (var file in request.Files)
                    {
                        var saveFileResult = await _customUploadFileService.UploadFileAsync("Notes", file);

                        entity.NoteFiles.Add(new NoteFile()
                        {
                            Id = Guid.NewGuid(),
                            FileName = saveFileResult.DirectoryPath
                        });
                    }
                }

                //Single File
                //if (request.File != null)
                //{
                //    var saveFileResult = await _customUploadFileService.UploadFileAsync(request.File, "Notes");

                //    entity.NoteFiles.Add(new NoteFile()
                //    {
                //        Id = Guid.NewGuid(),
                //        FileName = saveFileResult.DirectoryPath
                //    });
                //}

                await _applicationDbContext.Notes.AddAsync(entity, cancellationToken);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}