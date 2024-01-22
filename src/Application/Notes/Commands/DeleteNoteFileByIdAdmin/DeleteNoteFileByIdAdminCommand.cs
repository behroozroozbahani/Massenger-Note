using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PortalCore.Application.Notes.Commands.DeleteNoteFileByIdAdmin
{
    public class DeleteNoteFileByIdAdminCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteNoteFileByIdAdminCommandHandler : IRequestHandler<DeleteNoteFileByIdAdminCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DeleteNoteFileByIdAdminCommandHandler(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteNoteFileByIdAdminCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.NoteFiles
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            //Admin
            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (rol)
            {
                _applicationDbContext.NoteFiles.Remove(entity);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}
