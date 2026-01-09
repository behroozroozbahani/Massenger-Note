using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PortalCore.Application.Notes.Commands.DeleteNoteByIdAdmin
{
    public class DeleteNoteByIdAdminCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteNoteByIdAdminCommandHandler : IRequestHandler<DeleteNoteByIdAdminCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICustomUploadFileService _customUploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteNoteByIdAdminCommandHandler(IApplicationDbContext applicationDbContext, ICustomUploadFileService customUploadFileService, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _customUploadFileService = customUploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(DeleteNoteByIdAdminCommand request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .Include(x => x.NoteFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            //Admin
            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (rol)
            {
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
            }
            else
                throw new UnauthorizedAccessException();

            return Unit.Value;
        }
    }
}