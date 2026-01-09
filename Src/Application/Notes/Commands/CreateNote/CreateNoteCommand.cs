using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;

namespace PortalCore.Application.Notes.Commands.CreateNote
{
    public class CreateNoteCommand : IRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;

        public List<IFormFile>? Files { get; set; }

        //Single File
        //public IFormFile? Files { get; set; }
    }

    public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ICustomUploadFileService _customUploadFileService;

        public CreateNoteCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
        }

        public async Task<Unit> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Note>(request);

            //User Logged In
            if (entity.UserId == Guid.Parse("65c33454-fccc-4c48-228a-08d9c7a82ae1"))
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

                    //Single File
                    //if (request.Files != null)
                    //{
                    //    var saveFileResult = await _customUploadFileService.UploadFileAsync(request.Files, "Notes");
                    //    entity.NoteFiles.Add(new NoteFile()
                    //    {
                    //        Id = Guid.NewGuid(),
                    //        FileName = saveFileResult.DirectoryPath
                    //    });
                    //}

                    await _applicationDbContext.Notes.AddAsync(entity, cancellationToken);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}