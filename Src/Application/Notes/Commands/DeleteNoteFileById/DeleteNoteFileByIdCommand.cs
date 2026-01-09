using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;

namespace PortalCore.Application.Notes.Commands.DeleteNoteFileById
{
    public class DeleteNoteFileByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteNoteFileByIdCommandHandler : IRequestHandler<DeleteNoteFileByIdCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public DeleteNoteFileByIdCommandHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Unit> Handle(DeleteNoteFileByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.NoteFiles
                .Include(x => x.Note)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            if (entity.Note.UserId != Guid.Parse("8ec7224e-0e97-4937-069f-08d9e002a55f"))
            {
                throw new UnauthorizedAccessException();
            }

            _applicationDbContext.NoteFiles.Remove(entity);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}