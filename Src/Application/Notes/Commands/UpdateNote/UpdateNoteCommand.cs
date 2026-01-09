using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Domain.Entities;

namespace PortalCore.Application.Notes.Commands.UpdateNote
{
    public class UpdateNoteCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        //Multi File
        //public List<IFormFile>? Files { get; set; }

        //Single File
        public IFormFile? File { get; set; }
    }

    public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IMapper _mapper;

        public UpdateNoteCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ICustomUploadFileService customUploadFileService)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _customUploadFileService = customUploadFileService;
        }

        public async Task<Unit> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            if (entity.UserId != Guid.Parse("8ec7224e-0e97-4937-069f-08d9e002a55f"))
            {
                throw new UnauthorizedAccessException();
            }

            _mapper.Map(request, entity);

            //Multi File
            //if (request.Files != null && request.Files.Any())
            //{
            //    foreach (var file in request.Files)
            //    {
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
                var saveFileResult = await _customUploadFileService.UploadFileAsync("Notes", request.File);

                entity.NoteFiles.Add(new NoteFile()
                {
                    FileName = saveFileResult.DirectoryPath
                });
            }

            _applicationDbContext.Notes.Update(entity);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}