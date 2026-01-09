using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PortalCore.Application.Common.Exceptions;
using PortalCore.Application.Common.Interfaces;
using PortalCore.Application.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PortalCore.Application.Notes.Queries.GetNoteByIdAdmin
{
    public class GetNoteByIdAdminQuery : IRequest<NoteDto>
    {
        public Guid Id { get; set; }
    }

    public class GetNoteByIdAdminQueryHandler : IRequestHandler<GetNoteByIdAdminQuery, NoteDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetNoteByIdAdminQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NoteDto> Handle(GetNoteByIdAdminQuery request, CancellationToken cancellationToken)
        {
            var entity = await _applicationDbContext.Notes
                .Include(n => n.NoteFiles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException();

            var rol = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (rol != true)
            {
                throw new UnauthorizedAccessException();
            }

            return _mapper.Map(entity, new NoteDto());
        }
    }
}