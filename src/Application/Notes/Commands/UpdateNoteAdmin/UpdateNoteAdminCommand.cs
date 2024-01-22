using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalCore.Application.Notes.Commands.UpdateNoteAdmin
{
    public class UpdateNoteAdminCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        //Multi File
        //public List<IFormFile>? Files { get; set; }

        //Single File
        public IFormFile? File { get; set; }
    }

    public class UpdateNoteAdminCommandHandler : IRequestHandler<UpdateNoteAdminCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateNoteAdminCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(UpdateNoteAdminCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .Include(x => x.NoteFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            _mapper.Map(request, entity);

            //Admin
            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (rol)
            {
                //Multi File
                //if (request.Files != null && request.Files.Any())
                //{
                //    foreach (var file in request.Files)
                //    {
                //        if (entity.NoteFiles.Any())
                //        {
                //            foreach (var item in entity.NoteFiles)
                //            {
                //                if (!string.IsNullOrEmpty(item.FileName))
                //                {
                //                    _customUploadFileService.DeleteFile("Notes", item.FileName);
                //                }
                //            }
                //        }

                //        var saveFileResult = await _customUploadFileService.UploadFileAsync(file, "Notes");

                //        entity.NoteFiles.Add(new NoteFile()
                //        {
                //            Id = Guid.NewGuid(),
                //            FileName = saveFileResult.DirectoryPath
                //        });
                //    }
                //}

                //Single File
                if (request.File != null)
                {
                    if (entity.NoteFiles.Any())
                    {
                        foreach (var item in entity.NoteFiles)
                        {
                            if (!string.IsNullOrEmpty(item.FileName))
                            {
                                _customUploadFileService.DeleteFile("MessengerMessages", item.FileName);
                            }
                        }
                    }

                    var saveFileResult =
                        await _customUploadFileService.UploadFileAsync("MessengerMessages", request.File);
                    entity.NoteFiles.Add(new NoteFile()
                    {
                        Id = Guid.NewGuid(),
                        FileName = saveFileResult.DirectoryPath
                    });
                }

                _applicationDbContext.Notes.Update(entity);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}