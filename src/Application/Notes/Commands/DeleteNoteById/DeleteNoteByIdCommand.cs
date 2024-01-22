using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;

namespace PortalCore.Application.Notes.Commands.DeleteNoteById
{
    public class DeleteNoteByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;

        public DeleteNoteCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
        }

        public async Task<Unit> Handle(DeleteNoteByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            //User Logged In
            if (request.Id == entity.Id && entity.UserId != Guid.Parse("8ec7224e-0e97-4937-069f-08d9e002a55f"))
            {
                throw new UnauthorizedAccessException();
            }

            if (entity.NoteFiles.Any())
            {
                foreach (var file in entity.NoteFiles)
                {
                    _customUploadFileService.DeleteFile(file.FileName);
                    _applicationDbContext.NoteFiles.Remove(file);
                }
            }

            _applicationDbContext.Notes.Remove(entity);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}